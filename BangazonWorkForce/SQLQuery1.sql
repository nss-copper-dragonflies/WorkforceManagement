select* from Department
select * from employee

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