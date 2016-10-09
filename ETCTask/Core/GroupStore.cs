using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ETCTask.Models.Auth;

namespace ETCTask.Core
{
    public class GroupStore : IDisposable
    {
        private bool _disposed;
        private GroupStoreBase _groupStore;


        public GroupStore(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Context = context;
            _groupStore = new GroupStoreBase(context);
        }


        public IQueryable<Group> Groups
        {
            get { return _groupStore.EntitySet; }
        }

        public DbContext Context { get; private set; }

        public bool DisposeContext { get; set; }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public virtual void Create(Group group)
        {
            ThrowIfDisposed();
            if (group == null)
                throw new ArgumentNullException("role");
            _groupStore.Create(group);
            Context.SaveChanges();
        }


        public virtual async Task CreateAsync(Group group)
        {
            ThrowIfDisposed();
            if (group == null)
                throw new ArgumentNullException("role");
            _groupStore.Create(group);
            await Context.SaveChangesAsync();
        }


        public virtual async Task DeleteAsync(Group group)
        {
            ThrowIfDisposed();
            if (group == null)
                throw new ArgumentNullException("group");
            _groupStore.Delete(group);
            await Context.SaveChangesAsync();
        }


        public virtual void Delete(Group group)
        {
            ThrowIfDisposed();
            if (group == null)
                throw new ArgumentNullException("group");
            _groupStore.Delete(group);
            Context.SaveChanges();
        }


        public Task<Group> FindByIdAsync(string roleId)
        {
            ThrowIfDisposed();
            return _groupStore.GetByIdAsync(roleId);
        }


        public Group FindById(string roleId)
        {
            ThrowIfDisposed();
            return _groupStore.GetById(roleId);
        }

        public Task<Group> FindByNameAsync(string groupName)
        {
            ThrowIfDisposed();
            return _groupStore.EntitySet
                .FirstOrDefaultAsync(u => u.Name.ToUpper() == groupName.ToUpper());
        }


        public virtual async Task UpdateAsync(Group group)
        {
            ThrowIfDisposed();
            if (group == null)
                throw new ArgumentNullException("group");
            _groupStore.Update(group);
            await Context.SaveChangesAsync();
        }


        public virtual void Update(Group group)
        {
            ThrowIfDisposed();
            if (group == null)
                throw new ArgumentNullException("group");
            _groupStore.Update(group);
            Context.SaveChanges();
        }


        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (DisposeContext && disposing && (Context != null))
                Context.Dispose();
            _disposed = true;
            Context = null;
            _groupStore = null;
        }
    }
}