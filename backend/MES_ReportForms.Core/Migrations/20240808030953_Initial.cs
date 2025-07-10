using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MES_ReportForms.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "物料类型ID"),
                    CategoryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "商品类型编号"),
                    CategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "类型名称"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                },
                comment: "商品类型");

            migrationBuilder.CreateTable(
                name: "ChatChannel",
                columns: table => new
                {
                    ChannelId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "ChannelId"),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "用户ID"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "用户名"),
                    UserPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "电话号码"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "门店号"),
                    ShipToName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "门店名称"),
                    QueueMessageCount = table.Column<int>(type: "int", nullable: false, comment: "排队消息数"),
                    CustomerStatus = table.Column<int>(type: "int", nullable: true, comment: "客户的在线状态"),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "频道排队会话状态"),
                    SessionStartTime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "会话开始时间"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatChannel", x => x.ChannelId);
                },
                comment: "Chat频道");

            migrationBuilder.CreateTable(
                name: "ChatMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "ChannelId"),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "用户ID"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "用户名"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "门店号"),
                    UserType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "用户类型"),
                    ReadedMessageId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "已读的MessageId"),
                    ReadedMessageTime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "已读消息的时间"),
                    UnreadNumber = table.Column<int>(type: "int", nullable: true, comment: "未读数"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMember", x => x.Id);
                },
                comment: "Chat成员");

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    ChannelId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "ChannelId"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "消息内容"),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "用户ID"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "用户名"),
                    UserType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "用户类型"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "消息时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                },
                comment: "Chat成员");

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "客户ID"),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "客户编码"),
                    CustomerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "客户名称"),
                    CustomerEnName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "客户英文名称"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "电话号码"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerId);
                },
                comment: "客户");

            migrationBuilder.CreateTable(
                name: "CustomerMaterial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "物料ID"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "门店号"),
                    CustomerPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, comment: "客户单价"),
                    CanNonInventory = table.Column<bool>(type: "bit", nullable: true, comment: "是否允许无库存下单"),
                    ShowPrice = table.Column<bool>(type: "bit", nullable: true, comment: "是否显示单价"),
                    ShowInventory = table.Column<bool>(type: "bit", nullable: true, comment: "是否显示库存"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMaterial", x => x.Id);
                },
                comment: "客户的物料");

            migrationBuilder.CreateTable(
                name: "CustomerMOA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "门店号"),
                    MOAType = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "物料类型ID"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "物料ID"),
                    ConstraintQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "起送数量"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMOA", x => x.Id);
                },
                comment: "起始要求配置");

            migrationBuilder.CreateTable(
                name: "CustomerUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "用户ID"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "门店号"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerUser", x => x.Id);
                },
                comment: "客户门店与用户绑定关系");

            migrationBuilder.CreateTable(
                name: "DeliveryMethod",
                columns: table => new
                {
                    DeliveryMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "主键ID"),
                    MethodName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "配送方式"),
                    IsNeedSalesReview = table.Column<bool>(type: "bit", nullable: false, comment: "是否需要销售审核"),
                    DeliveryTime = table.Column<int>(type: "int", nullable: true, comment: "配送时效"),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, comment: "配送费用"),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryMethod", x => x.DeliveryMethodId);
                },
                comment: "仓库");

            migrationBuilder.CreateTable(
                name: "FixedOrderRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "固定订单申请ID"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "门店号"),
                    CustomerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "客户名称"),
                    Config = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true, comment: "配置内容（程序内定义的JSON文本内容）"),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "备注"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedOrderRequest", x => x.Id);
                },
                comment: "固定订单申请");

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "组ID"),
                    GroupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "组名"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.GroupId);
                },
                comment: "组");

            migrationBuilder.CreateTable(
                name: "GroupMenu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "菜单ID"),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "组ID"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMenu", x => x.Id);
                },
                comment: "用户组分配的菜单权限");

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "仓库ID"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "物料ID"),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, comment: "当前库存量"),
                    WareHouse = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "仓库"),
                    LatestTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "最后更新时间"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                },
                comment: "仓库");

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "物料ID"),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "物料编码"),
                    MaterialEnName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "物料英文名称"),
                    MaterialZhName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "物料中文名称"),
                    Epithet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "别称，可多个别称，逗号分割"),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "计量单位"),
                    Specifications = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "规格"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "单价"),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, comment: "物料类型代码"),
                    Picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "图片URL"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "物料描述"),
                    IsGlobal = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "是否全局"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.MaterialId);
                },
                comment: "物料");

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "菜单ID"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "名称"),
                    Event = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "触发事件(前端应用是路由地址，非Web应用可以定义事件Key)"),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "图标样式(如fa fa-cog)"),
                    Short = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "简称"),
                    Sort = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "排序"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "newid()", comment: "菜单ID"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.MenuId);
                },
                comment: "菜单");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "表主键"),
                    OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "订单号"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "客户ID"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "门店号"),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "备注"),
                    OrderAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "订单金额"),
                    OrderStatus = table.Column<int>(type: "int", nullable: true, defaultValue: 10, comment: "订单状态"),
                    OrderType = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "订单类型"),
                    OrderSource = table.Column<int>(type: "int", nullable: true, comment: "订单来源"),
                    CloseDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "订单关闭日期"),
                    RDDId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: true, comment: "RDDId"),
                    IsUrgent = table.Column<bool>(type: "bit", nullable: true, defaultValue: false, comment: "是否为紧急订单"),
                    UrgentLogistics = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: true, comment: "紧急订单配送方式"),
                    UrgentRDD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "紧急订单指定车期"),
                    FixedOrderRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "固定订单申请ID"),
                    CustomerPoNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "客户的采购单号"),
                    SAPID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "对应SAP订单标识"),
                    OperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "作业人员ID"),
                    IssueFlag = table.Column<int>(type: "int", nullable: true, comment: "问题标识"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.UniqueConstraint("AK_Order_OrderNo", x => x.OrderNo);
                },
                comment: "订单");

            migrationBuilder.CreateTable(
                name: "OrderLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "订单号"),
                    LogType = table.Column<int>(type: "int", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "备注"),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLog", x => x.Id);
                },
                comment: "订单日志");

            migrationBuilder.CreateTable(
                name: "Quota",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "配额ID"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "物料ID"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "门店号"),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "数量"),
                    Status = table.Column<int>(type: "int", nullable: true, defaultValue: 0, comment: "配额状态"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quota", x => x.Id);
                },
                comment: "配额");

            migrationBuilder.CreateTable(
                name: "RDDData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    ConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "车期配置ID"),
                    NextDeliveryDay = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "车期日期"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RDDData", x => x.Id);
                },
                comment: "车期数据");

            migrationBuilder.CreateTable(
                name: "RequiredDeliveryDate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "车期ID"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "门店号"),
                    RouteNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "线程编号"),
                    RouteName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "线路名称"),
                    CategoryId = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "线路类型Id"),
                    Plant = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Plant"),
                    AutoCreateSchedule = table.Column<bool>(type: "bit", nullable: false, comment: "自动创建车期"),
                    Config = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true, comment: "配置内容（程序内定义的JSON文本内容）"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequiredDeliveryDate", x => x.Id);
                },
                comment: "车期");

            migrationBuilder.CreateTable(
                name: "ReturnOrder",
                columns: table => new
                {
                    ReturnOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    ShipToNumber = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "门店号"),
                    OrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "换货订单号"),
                    CustomerPoNumber = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "PO.No"),
                    ReturnOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "退货单号"),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "备注"),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "状态"),
                    Classification = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "退换货类型（1:退货 2:换货）"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnOrder", x => x.ReturnOrderId);
                },
                comment: "退换货物料");

            migrationBuilder.CreateTable(
                name: "ReturnOrderLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    ReturnOrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "退换货订单号"),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "备注"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnOrderLog", x => x.Id);
                },
                comment: "退换货订单日志");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "用户ID"),
                    Password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "密码"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "用户登录账号"),
                    Photo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "相片地址"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "电话号码"),
                    UserType = table.Column<int>(type: "int", nullable: false, comment: "用户类型"),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否被禁用"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                },
                comment: "用户");

            migrationBuilder.CreateTable(
                name: "ShipTo",
                columns: table => new
                {
                    ShipToNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "门店号"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "客户ID"),
                    ShipToName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "门店名称"),
                    ShipToEnName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "门店英文名称"),
                    ShipToAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "门店地址"),
                    ShipToEnAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "门店英文地址"),
                    CanNonInventory = table.Column<bool>(type: "bit", nullable: false, comment: "是否允许无库存下单"),
                    ShowPrice = table.Column<bool>(type: "bit", nullable: false),
                    ShowInventory = table.Column<bool>(type: "bit", nullable: false, comment: "是否显示库存"),
                    MaterialConstraintType = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "物料约束类型（1:全部物料有效，但如有配置的物料规则，规则优先。 2:仅配置物料的有效）"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipTo", x => x.ShipToNumber);
                    table.ForeignKey(
                        name: "FK_ShipTo_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "门店");

            migrationBuilder.CreateTable(
                name: "FixedOrderMaterial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, comment: "固定订单申请ID"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "物料商品ID"),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "物料数量"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedOrderMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixedOrderMaterial_FixedOrderRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "FixedOrderRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "固定订单申请的物料清单");

            migrationBuilder.CreateTable(
                name: "OrderMaterial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "订单商品明细ID"),
                    OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "订单号"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "物料ID"),
                    MaterialName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "物料名称"),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, comment: "原价"),
                    RealPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, comment: "实际售价"),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "数量"),
                    Classification = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "正常下单的物料（1:正常下单的物料 2：配额添加的物料 3:促销）"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderMaterial_Order_OrderNo",
                        column: x => x.OrderNo,
                        principalTable: "Order",
                        principalColumn: "OrderNo",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "订单物料商品");

            migrationBuilder.CreateTable(
                name: "ReturnMaterial",
                columns: table => new
                {
                    ReturnMaterialId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    ReturnOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "退/换货主表外键"),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "物料ID"),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "退/换货数量"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnMaterial", x => x.ReturnMaterialId);
                    table.ForeignKey(
                        name: "FK_ReturnMaterial_Material_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Material",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReturnMaterial_ReturnOrder_ReturnOrderId",
                        column: x => x.ReturnOrderId,
                        principalTable: "ReturnOrder",
                        principalColumn: "ReturnOrderId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "退换货物料");

            migrationBuilder.CreateTable(
                name: "GroupUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()", comment: "ID"),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "组ID"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "用户ID"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "是否已删除"),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "", comment: "创建用户ID"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()", comment: "创建时间"),
                    ModifierUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "修改用户ID"),
                    ModifierDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "修改时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户与组关联");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMember_ChannelId",
                table: "ChatMember",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMember_UserId",
                table: "ChatMember",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChannelId",
                table: "ChatMessage",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedOrderMaterial_RequestId",
                table: "FixedOrderMaterial",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_UserId",
                table: "GroupUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterial_OrderNo",
                table: "OrderMaterial",
                column: "OrderNo");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterial_MaterialId",
                table: "ReturnMaterial",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnMaterial_ReturnOrderId",
                table: "ReturnMaterial",
                column: "ReturnOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipTo_CustomerId",
                table: "ShipTo",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "ChatChannel");

            migrationBuilder.DropTable(
                name: "ChatMember");

            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "CustomerMaterial");

            migrationBuilder.DropTable(
                name: "CustomerMOA");

            migrationBuilder.DropTable(
                name: "CustomerUser");

            migrationBuilder.DropTable(
                name: "DeliveryMethod");

            migrationBuilder.DropTable(
                name: "FixedOrderMaterial");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "GroupMenu");

            migrationBuilder.DropTable(
                name: "GroupUser");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "OrderLog");

            migrationBuilder.DropTable(
                name: "OrderMaterial");

            migrationBuilder.DropTable(
                name: "Quota");

            migrationBuilder.DropTable(
                name: "RDDData");

            migrationBuilder.DropTable(
                name: "RequiredDeliveryDate");

            migrationBuilder.DropTable(
                name: "ReturnMaterial");

            migrationBuilder.DropTable(
                name: "ReturnOrderLog");

            migrationBuilder.DropTable(
                name: "ShipTo");

            migrationBuilder.DropTable(
                name: "FixedOrderRequest");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "ReturnOrder");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
