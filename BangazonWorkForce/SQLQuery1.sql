select* from Department
select * from employee
select * from computer
select id, Make, Manufacturer, PurchaseDate 
from computer
where Id = 1

select e.FirstName, e.LastName, d.[name], d.budget
from Employee e left join department d on e.departmentId = d.id
where e.departmentId = d.id

select * Into employee
from Department
where employee.Id = Department.Id

SELECT d.Id,d.[name], d.Budget, e.FirstName, e.LastName
FROM Department d 
join Employee e
ON e.DepartmentId = d.Id
where d.id = 3

SELECT d.Id, count(e.Id) as DepartmentSize, d.[name], d.Budget
                                        FROM Department d join Employee e
                                        ON e.DepartmentId = d.Id
                                        group by d.Id, d.Name, d.Budget


										SELECT d.Id, count(e.Id) as DepartmentSize, d.[name], d.Budget
                                        FROM Department d join Employee e
                                        ON e.DepartmentId = d.Id
                                        group by d.Id, d.Name, d.Budget

										SELECT d.Id, d.[name], d.Budget, e.Id as employeeId, e.FirstName
                                        FROM Department d left join Employee e
                                        ON e.DepartmentId = d.Id

INSERT INTO Computer (PurchaseDate, Make, Manufacturer)
VALUES ('2019/04/12', 'Surface', 'Microsoft')

delete from computer where id = @id 
        and not exists(select EmployeeId from [ComputerEmployee] where EmployeeId = @id)