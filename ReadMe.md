# Mindex Coding Challenge
## What's Provided
A simple [.Net 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) web application has been created and bootstrapped 
with data. The application contains information about all employees at a company. On application start-up, an in-memory 
database is bootstrapped with a serialized snapshot of the database. While the application runs, the data may be
accessed and mutated in the database without impacting the snapshot.

### How to Run
You can run this by executing `dotnet run` on the command line or in [Visual Studio Community Edition](https://www.visualstudio.com/downloads/).


### How to Use
The following Employee endpoints are available to use:
```
* CREATE
    * HTTP Method: POST 
    * URL: localhost:8080/api/employee
    * PAYLOAD: Employee
    * RESPONSE: Employee
* READ
    * HTTP Method: GET 
    * URL: localhost:8080/api/employee/{id}
    * RESPONSE: Employee
* UPDATE
    * HTTP Method: PUT 
    * URL: localhost:8080/api/employee/{id}
    * PAYLOAD: Employee
    * RESPONSE: Employee
* READ
    * HTTP Method: GET
    * URL: localhost:8080/api/employee/reportingStructure/{id}
    * PAYLOAD: Employee
    * RESPONSE: ReportingStructure
```
The Employee has a JSON schema of:
```json
{
  "type":"Employee",
  "properties": {
    "employeeId": {
      "type": "string"
    },
    "firstName": {
      "type": "string"
    },
    "lastName": {
          "type": "string"
    },
    "position": {
          "type": "string"
    },
    "department": {
          "type": "string"
    },
    "managerId": {
          "type": "string"
    },
    "directReports": {
      "type": "array",
      "items": "string"
    }
  }
}
```
The Reporting Structure has a JSON schema of:
```json
{
  "type": "ReportingStructure",
  "properties": {
    "employee": {
      "type": "Employee"
    },
    "numberOfReports": {
      "type": "integer"
    }
  }
}
```
The following Compensation endpoints are available to use:
```
* CREATE
    * HTTP Method: POST 
    * URL: localhost:8080/api/compensation
    * PAYLOAD: Compensation
    * RESPONSE: Compensation
* READ
    * HTTP Method: GET 
    * URL: localhost:8080/api/compensation/{id}
    * RESPONSE: Compensation
```

The Compensation has a JSON schema of:
```json
{
  "type": "Compensation",
  "properties": {
    "employee": {
      "type": "Employee"
    },
    "salary": {
      "type": "decimal"
    },
    "effectivcDate": {
      "type": "DateTIme"
    }
  }
}
```
For all endpoints that require an "id" in the URL, this is the "employeeId" field.

## What I Implemented
I implemented both task 1 and task 2. Also, I made changes to the EmployeeControllerTest class as well as the Create endpoint for employee. To do so, I added a new ManagerId field to Employee as well. 

### Task 1 - COMPLETED
Create a new type, ReportingStructure, that has two properties: employee and numberOfReports.

For the field "numberOfReports", this should equal the total number of reports under a given employee. The number of 
reports is determined to be the number of directReports for an employee and all of their direct reports. For example, 
given the following employee structure:
```
                    John Lennon
                /               \
         Paul McCartney         Ringo Starr
                               /        \
                          Pete Best     George Harrison
```
The numberOfReports for employee John Lennon (employeeId: 16a596ae-edd3-4847-99fe-c4518e82c86f) would be equal to 4. 

This new type should have a new REST endpoint created for it. This new endpoint should accept an employeeId and return 
the fully filled out ReportingStructure for the specified employeeId. The values should be computed on the fly and will 
not be persisted.

### Task 2 - COMPLETED
Create a new type, Compensation. A Compensation has the following fields: employee, salary, and effectiveDate. Create 
two new Compensation REST endpoints. One to create and one to read by employeeId. These should persist and query the 
Compensation from the persistence layer.

## Delivery
Uploaded to git repo and shared the public repo to Mark Dupont.
