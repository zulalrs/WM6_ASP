using Admin.DAL;
using Admin.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.BLL.Repository
{
    public abstract class RepositoryBase<T, TId> : IDisposable where T : BaseEntity<TId>
    {
        internal static MyContext DbContext;
        private static DbSet<T> DbObject;
        protected RepositoryBase()
        {
            DbContext = DbContext ?? new MyContext();
            TimeSpan dd = DateTime.Now - DbContext.InstanceDate;
            if (IsDisposed) DbContext = new MyContext();    // Dispose oldugu zaman tekrar contextten instance alarak contextimizi yeniliyoruz.
            if (dd.TotalMinutes > 30) DbContext = new MyContext();  // 30 dk da bir instance ı yenileme
            DbObject = DbContext.Set<T>();
        }
        // *** GetAll
        public List<T> GetAll()
        {
            return DbObject.ToList();
        }
        public List<T> GetAll(Func<T, bool> predicate)  // Şarta bağlı olarak nesneleri getirme
        {
            return DbObject.Where(predicate).ToList();
        }
        public async Task<List<T>> GetAllAsync()    
        {
            return await DbObject.ToListAsync();
        }
        public async Task<List<T>> GetAllAsync(Func<T, bool> predicate)
        {
            return await DbObject.Where(predicate).AsQueryable().ToListAsync();
        }

        // *** GetById
        public T GetById(params object[] keys)  // Bir tabloda birden fazla key olma durumu oldugu için ve aynı zamanda Find metodu parametresi birden fazla parametre alımını desteklediği için bu metodumuzu boyle yazdık. (Computed tablolarda iki key kullanılıyor.)
        {
            return DbObject.Find(keys);
        }
        public async Task<T> GetByIdAsync(params object[] keys)
        {
            return await DbObject.FindAsync(keys);
        }

        // *** Insert
        public int Insert(T entity)
        {
            DbObject.Add(entity);
            return DbContext.SaveChanges();
        }
        public void InsertForMark(T entity) // For dongusu içerisinde bu metodu çalıştırırsak sürekli olarak nesnelerle içini dolduracak fakat değişiklikleri kaydetmeycek. Değişikliklerin kaydolması için bu işlemden sonra sadece SaveChanges in içinde oldugu metodu çalıştırmalıyız.
        {
            DbObject.Add(entity);
        }
        public async Task<int> InsertAsync(T entity)
        {
            DbObject.Add(entity);
            return await DbContext.SaveChangesAsync();
        }

        // *** Delete
        public int Delete(T entity)
        {
            DbObject.Remove(entity);
            return DbContext.SaveChanges();
        }
        public void DeleteForMark(T entity)
        {
            DbObject.Remove(entity);
        }
        public async Task<int> DeleteAsync(T entity)
        {
            DbObject.Remove(entity);
            return await DbContext.SaveChangesAsync();
        }

        // *** Save
        public int Save()
        {
            return DbContext.SaveChanges();
        }
        public async Task<int> SaveAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        // *** Update
        public int Update(T entity) // Entity i güncelleme. EntityState ini Modified yaparsak guncellendiğini anlıyor.
        {
            DbObject.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
            entity.UpdatedDate = DateTime.Now;
            return this.Save();
        }

        public IQueryable<T> Queryable()
        {
            return DbObject;
        }
        public bool IsDisposed { get; set; }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.IsDisposed = true;
        }
    }
}
