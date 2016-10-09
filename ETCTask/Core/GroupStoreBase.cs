using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ETCTask.Models.Auth;

namespace ETCTask.Core
{
    public class GroupStoreBase
    {
        public GroupStoreBase(DbContext context)
        {
            Context = context;
            DbEntitySet = context.Set<Group>();
        }

        public DbContext Context { get; }


        public DbSet<Group> DbEntitySet { get; }


        public IQueryable<Group> EntitySet
        {
            get { return DbEntitySet; }
        }


        public void Create(Group entity)
        {
            DbEntitySet.Add(entity);
        }


        public void Delete(Group entity)
        {
            DbEntitySet.Remove(entity);
        }


        public virtual Task<Group> GetByIdAsync(object id)
        {
            return DbEntitySet.FindAsync(id);
        }


        public virtual Group GetById(object id)
        {
            return DbEntitySet.Find(id);
        }


        public virtual void Update(Group entity)
        {
            if (entity != null)
                Context.Entry(entity).State = EntityState.Modified;
        }
    }
}