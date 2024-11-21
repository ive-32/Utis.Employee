## Тестовое задание

Написать сервер для хранения данных сотрудников.
Реализовать клиента CRUD.
Также реализовать обновление данных по контраку  

```protobuf
service WorkerIntegration {
	rpc GetWorkerStream (EmptyMessage) returns (stream WorkerAction);
}

message WorkerAction
{
	WorkerMessage worker = 1;
	Action actionType = 2;
}

message WorkerMessage
{
	string LastName = 1;
	string FirstName = 2;
	string MiddleName = 3;
	int64 Birthday = 4;
	Sex Sex = 5;
	bool HaveChildren = 6;
	google.protobuf.Int64Value Id = 7;
}
```

## Employee.RpcService
Сервис для хранения данных 
gRPC - контракт описан в EmployeeService.Proto

```protobuf
service EmployeeService {
  rpc Create (WorkerMessage) returns (IdModel);
  rpc Update (WorkerMessage) returns (EmptyMessage);
  rpc Delete (IdModel) returns (EmptyMessage);
  rpc GetList (GetListModel) returns (WorkerMessages);
}
```

## Employee.Data
Данные, codeFirst и контекст вынесем в отдельный проект 
Бд Postgres

## Employee.Proto
Прото контракты, в отдельном проекте. Собираются в nuget для переиспользования в друих проектах

## Employee.WorkerHost 
Воркер для полученя данных из WorkerIntegration 
вызывает метод GetWorkerStream с периодичностью указанной в конфигурации, <br>
Для сохранения вызывает методы Employee.RpcService.
Не стал делать множественное обновление а обновляю по одной записи вот почему <br>
Количество сотрудников - 5000 для крупного предприятия. В самом эксремальном случае прилетят все 5000.<br>
Проще и надежнее по одной записи обрабатывать, базу не блокировать и код лишний не генерировать.<br>
Для другого сценария когда поток данных будет больше, лучше собирать группами Создание Обновление Удаление.<br>
Фильровать и убирать дублирующиеся операции над одним сотрудником и массово обновлять удалять создавать

<br>
Предоставленный контракт кислый - не предполагает обратной отсылки ошибок и результатов. 
Поэтому все ошибки - в лог.<br>
Считаю, что лучше, если WorkerIntegration инициирует запрос поточного обновления, и в ответе получет результаты исполнения. 
<br>
Но раз есть такой контракт - то делаем по нему.


## Employee.Client 
Клиент на blazor. Не писал раньше клиентские части. Поэтому тут казни египетские. <br/>
Умеет показать список, внести изменения в карточку, отфильтровать по фамилии.<br>
Не хватает пагинации - достает только первые 50 записей. Employee.RpcService поддерживает пагинацию

## WorkerIntegrationMoq
Симулятор WorkerIntegration сервиса, генерирует очередь с WorkerActions. Реализует GetWorkerStream - отправляет записи из этой очереди.
Для генерации WorkerAction запрашивает GetList из Employee.RpcService чтобы обновлять и удалять сотрудников.