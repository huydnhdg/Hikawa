﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HikawaEntities : DbContext
    {
        public HikawaEntities()
            : base("name=HikawaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Accessary_Big> Accessary_Big { get; set; }
        public virtual DbSet<Accessary_Export> Accessary_Export { get; set; }
        public virtual DbSet<Accessary_Import> Accessary_Import { get; set; }
        public virtual DbSet<Accessary_Key> Accessary_Key { get; set; }
        public virtual DbSet<Acessary_Export_Item> Acessary_Export_Item { get; set; }
        public virtual DbSet<Acessary_Log_Key> Acessary_Log_Key { get; set; }
        public virtual DbSet<Agent_Bonus> Agent_Bonus { get; set; }
        public virtual DbSet<Agent_Log_Active> Agent_Log_Active { get; set; }
        public virtual DbSet<Agent_Log_Payment> Agent_Log_Payment { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Article_Cate> Article_Cate { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<B_Cate> B_Cate { get; set; }
        public virtual DbSet<B_Log_Active> B_Log_Active { get; set; }
        public virtual DbSet<B_Model> B_Model { get; set; }
        public virtual DbSet<B_Model_Process> B_Model_Process { get; set; }
        public virtual DbSet<B_Payment> B_Payment { get; set; }
        public virtual DbSet<B_Process> B_Process { get; set; }
        public virtual DbSet<B_Voucher> B_Voucher { get; set; }
        public virtual DbSet<Config_Bonus> Config_Bonus { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<E_OderItems> E_OderItems { get; set; }
        public virtual DbSet<E_Order> E_Order { get; set; }
        public virtual DbSet<E_Order_Log> E_Order_Log { get; set; }
        public virtual DbSet<E_Product> E_Product { get; set; }
        public virtual DbSet<E_Product_Extra> E_Product_Extra { get; set; }
        public virtual DbSet<E_Product_Image> E_Product_Image { get; set; }
        public virtual DbSet<E_ProductCate> E_ProductCate { get; set; }
        public virtual DbSet<Log_Action> Log_Action { get; set; }
        public virtual DbSet<LOG_MO> LOG_MO { get; set; }
        public virtual DbSet<Model_Price> Model_Price { get; set; }
        public virtual DbSet<Move_Price> Move_Price { get; set; }
        public virtual DbSet<P_Export> P_Export { get; set; }
        public virtual DbSet<P_Export_Item> P_Export_Item { get; set; }
        public virtual DbSet<P_Import> P_Import { get; set; }
        public virtual DbSet<P_Import_Item> P_Import_Item { get; set; }
        public virtual DbSet<P_Model> P_Model { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCate> ProductCates { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Repair_Price> Repair_Price { get; set; }
        public virtual DbSet<SendCode> SendCodes { get; set; }
        public virtual DbSet<SentNotifi> SentNotifis { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Type_Err> Type_Err { get; set; }
        public virtual DbSet<User_Device> User_Device { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }
        public virtual DbSet<Warranti> Warrantis { get; set; }
        public virtual DbSet<Warranti_Accessary> Warranti_Accessary { get; set; }
        public virtual DbSet<Warranti_Image> Warranti_Image { get; set; }
        public virtual DbSet<Warranti_Log> Warranti_Log { get; set; }
        public virtual DbSet<Service_Price> Service_Price { get; set; }
        public virtual DbSet<AspController> AspControllers { get; set; }
        public virtual DbSet<AspRoleController> AspRoleControllers { get; set; }
        public virtual DbSet<LOG_Topup> LOG_Topup { get; set; }
        public virtual DbSet<ProductStamp> ProductStamps { get; set; }
    }
}
