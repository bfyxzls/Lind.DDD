# Lind.DDD.Manager
* 一个通用的后台管理系统，在它基础上开发新的功能
* 首先修改web.config里的sql连接串，建立自己的数据库
* 项目启动之后自动建立数据表，初始化数据，用户名admin，密码admin

# 实体数据表添加
在Models文件夹里添加你的实体表，与数据表同名，继承`Lind.DDD.Domain.EntityBase`，之后享受公用的属性字段
```
    /// <summary>
    /// 评价表
    /// </summary>
    public class Evaluation : Lind.DDD.Domain.EntityBase
    {
        /// <summary>
        /// 标识列
        /// </summary>
        [DisplayName("编号"), Column("ID"), Required, Key]
        public int Id { get; set; }
        /// <summary>
        /// 星级
        /// </summary>
        [DisplayName("星级")]
        public int Star { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [DisplayName("内容")]
        public String Content { get; set; }
        /// <summary>
        /// 评价人ID
        /// </summary>
        [DisplayName("评价人ID")]
        public int evaluateUserId { get; set; }
        /// <summary>
        /// 评价人名字
        /// </summary>
        [DisplayName("评价人名字")]
        public String evaluateName { get; set; }
    }
```

# 将实体更新到数据库
在添加数据实体之后，我们可以通过迁移工具migrations将实体更新到数据库里
```
enable-migrations
add migrations 20200525
update-database
```