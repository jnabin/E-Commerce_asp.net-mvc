using E_Commerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace E_Commerce.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected EcommerceEntities context = new EcommerceEntities();
        public Customer validateLogin(string username, string password) {
            if(context.Set<Customer>().FirstOrDefault(c => c.Username == username && c.password == password) != null)
            {
                return context.Set<Customer>().FirstOrDefault(c => c.Username == username && c.password == password);
            }
            else
            {
                return null;
            }
            
        }

        public Admin adminValidateLogin(string username, string password)
        {
            if (context.Set<Admin>().FirstOrDefault(c => c.Username == username && c.Password == password) != null)
            {
                return context.Set<Admin>().FirstOrDefault(c => c.Username == username && c.Password == password);
            }
            else
            {
                return null;
            }

        }
        public bool GetByName(string name)
        {
            if(context.Set<Customer>().FirstOrDefault(c => c.Username == name) != null  || context.Set<Admin>().FirstOrDefault(c => c.Username == name) != null || context.Set<Manager>().FirstOrDefault(c => c.Username == name) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public List<OrderProduct> GetByCustomerId(int cid, int pid)
        {
            return context.Set<OrderProduct>().Where(x=> x.CustomerID == cid && x.ProductID == pid).ToList();
        }

        public bool GetByProductName(string name, int id)
        {
            if (context.Set<Product>().FirstOrDefault(c => c.CategoryID == id) != null && context.Set<Product>().FirstOrDefault(c => c.Product_name == name) != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public ProductHistory GetByProductNameCategory(string name, int id)
        {
            return context.Set<ProductHistory>().Where(x => x.CategoryID==id && x.Product_name == name).FirstOrDefault();
            
        }

        public List<Product> GetfromFinalCategory(int fid)
        {
            return context.Set<Product>().Where(p=>p.FinalSubCategoryID==fid).ToList();
        }

        public List<Product> GetfromSubCategory(int sid)
        {
            return context.Set<Product>().Where(p => p.SubCategoryID == sid).ToList();
        }

        public List<Product> GetfromMainCategory(int mid)
        {
            return context.Set<Product>().Where(p => p.CategoryID == mid).ToList();
        }

        public int GetMainCategaoryID(int id)
        {
            return id;
        }
        public void Delete(int id)
        {
            context.Set<TEntity>().Remove(Get(id));
            context.SaveChanges();
        }

        public void DeleteSize(int id)
        {
            context.Set<ProductSize>().RemoveRange(context.Set<ProductSize>().Where(x => x.ProductID==id));
            context.SaveChanges();
        }

        public TEntity Get(int id)
        {
            return context.Set<TEntity>().Find(id);
        }

        public List<Product> ProductsWithoutSize()
        {
            var v = context.Set<Product>().Where(x => x.SizeCategory != "others");
            return v.Where(x=>x.ProductSizes.Count==0).ToList();
        }

        public List<TEntity> GetAll()
        {
            return context.Set<TEntity>().ToList();
        }

        public void Insert(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException exc)
            {

                throw exc;
            }
            
        }

        public void Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        private void ProductUpdate(Product product)
        {
            context.Entry(product).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void PartialUpdate(Product product)
        {
            var item = context.Set<Product>().Find(product.Product_id);
            item.ImageFile = product.ImageFile;
            item.Description = product.Description;
            item.UnitPrice = product.UnitPrice;
            item.Product_name = product.Product_name;
            ProductUpdate(item);

        }

        private void ProductHistoryUpdate(ProductHistory product)
        {
            context.Entry(product).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void PartialHistoryUpdate(ProductHistory product)
        {
            var item = context.Set<ProductHistory>().Find(product.Product_id);
            item.ImageFile = product.ImageFile;
            item.Description = product.Description;
            item.UnitPrice = product.UnitPrice;
            item.Product_name = product.Product_name;
            ProductHistoryUpdate(item);

        }
    }
}