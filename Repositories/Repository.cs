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

        public TEntity Get(int id)
        {
            return context.Set<TEntity>().Find(id);
        }

        public List<TEntity> GetAll()
        {
            return context.Set<TEntity>().ToList();
        }

        public void Insert(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            context.SaveChanges();
            
        }

        public void Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}