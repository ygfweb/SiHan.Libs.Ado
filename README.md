# SiHan.Libs.Ado
## 介绍

本库是ADO.NET的扩展，提供常用的CRUD（增、删、改、查）操作，使用标准的SQL语句，兼容目前主流的数据库。其内部采用一个支持多线程的反射缓存池，可以高效便捷的操作数据。

该项目基于.Net Standard 2.0，无任何依赖。

## 辅助类

- BaseEntity：抽象实体类，所有实体必须从该类继承，该类不提供任何功能，仅用于泛型约束。

- BaseValueConvert：值转换抽象类，用于列的数据类型与实体属性类型不兼容时，提供自定义的数据转换方式。

- GuidValueConvert：GUID值转换器，该类是BaseValueConvert类的实现，提供string与guid之间的转换。

- MapScheme：映射方案枚举，当操作MySQL、PostgreSQL等数据库，它们的表名、列名不区分大小写，通过设置默认映射方案，可以避免为实体类分配特性，指定不同名称的繁琐工作。如果实体类通过特性指定了其数据成员的名称，将自动覆盖默认映射方案。其值为：
  - OriginalName（默认值：原始名称）：即类名、属性名做为表名、列名。
  - UnderScoreCase（小写下划线）：自动将类名、属性名转换成小写下划线，其作为表名、列名。

## 特性类

- TableAttribute：类特性，用于指定实体类相应的表名，当表名与类名不一致时，需要使用该特性。
  - Name：表名，如果为空，将使用默认映射规则。

- ColumnAttribute：用于指定属性对应的列名，当属性名称与数据库中的列名不同时，需要使用该特性。
  - Name：数据列的名称，如果为空，将使用默认映射规则。
  - Convert：值转换器，必须是BaseValueConvert继承类。

- KeyAttribute：主键特性，指定做为数据库主键的属性，本库对主键的判断规则，当类属性名称为Id或使用该特性装饰时，即为主键。本库不支持复合主键。
  - IsAuto：是否为自动增量，如果为true，则该列将不包含在插入语句中。

- IgnoreAttribute：忽略特性，使用该特性装饰的属性表示不与数据库进行映射。

## 扩展库



- DbCommandExtensions：为DbCommand添加了两个扩展方法：
  - AppendAnonymousParameters：使用匿名对象为DbCommand对象添加参数。
  - AppendEntityParameters：使用实体类为为DbCommand对象添加参数，将使用实体类的映射规则。
- DbDataReaderExtensions：为DbDataReader添加了以下扩展方法：
  - ToEntity：使用映射规则将DbDataReader转换成实体类。
- DbConnectionExtensions：该类是本库的核心类，提供了大多数功能：
  - DefaultMapScheme：静态属性，指定系统采用的默认映射方案。
  - SelectAsync()：扩展方法，将SQL语句的查询结果转换成实体列表。
  - GetAllAsync()：扩展方法，获取表中的所有记录。
  - SingleByIdAsync()：通过ID值获取唯一的实体对象，若不存在，则返回NULL。
  - FirstOrDefaultAsync()：获取SQL查询结果的第一条记录，并转换成实体，如果不存在，则返回NULL。
  - ScalarAsync()：标量查询。
  - ExecuteNonQueryAsync()：执行非查询的SQL语句，可接收匿名对象做为SQL语句的参数。
  - InsertAsync()：向数据库中插入单条、多条记录，如果是批量插入，必须传入第2个参数，即事务。
  - DeleteAsync()：删除单条、多条记录，如果是批量插入，必须传入第2个参数，即事务。
  - DeleteByIdAsync()：通过ID值删除记录。
  - DeleteAllAsync()：删除数据表中的所有记录。
  - UpdateAsync()：更新单条、多条记录，如果是批量插入，必须传入第2个参数，即事务。

## 使用例子

### 实体类

```c#
    public class Account : BaseEntity
    {
        [Column(Convert = typeof(GuidValueConvert))]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LoginTime { get; set; }
        public decimal Money { get; set; }
    }
```

### 数据操作

```c#
 using (var connection = DbFactory.Create())
 {
     connection.Open();
     Account account = new Account
     {
     	Id = Guid.NewGuid().ToString(),
        CreatedTime = DateTime.Now,
        Email = "user@gmail.com",
        LoginTime = DateTime.Now,
        Money = 15,
        Password = "123",
        UserName = "user"
       };
       await connection.InsertAsync(account);
   }
```

