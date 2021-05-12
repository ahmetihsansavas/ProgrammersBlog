using ProgrammersBlog.DataAccess.Abstract;
using ProgrammersBlog.DataAccess.Concrete.EntityFramework.Contexts;
using ProgrammersBlog.DataAccess.Concrete.EntityFramework.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.DataAccess.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProgrammersBlogContext _context;
        private EfArticleRepository _articleRepository;
        private EfCategoryRepository _categoryRepository;
        private EfCommentRepository _commentRepository;
        private EfRoleRepository _roleRepository;
        private EfUserRepository _userRepository;
        public UnitOfWork(ProgrammersBlogContext context)
        {
            _context = context;
        }
        //IArticleRepository üz. Articles'a ulaş. zaman _articleRepository null değilse ulaş null ise yeni bir EfArticleRepo. oluşt.
        public IArticleRepository Articles => _articleRepository ?? new EfArticleRepository(_context);

        public ICategoryRepository Categories => _categoryRepository ?? new EfCategoryRepository(_context);


        public ICommentRepository Comments => _commentRepository ?? new EfCommentRepository(_context);


        public IRoleRepository Roles => _roleRepository ?? new EfRoleRepository(_context);


        public IUserRepository Users => _userRepository ?? new EfUserRepository(_context);

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
            // IDisposable ve dosya ve kanal tutamaçları, kayıt defteri tutamaçları, bekleme tutamaçları veya yönetilmeyen bellek bloklarına yönelik işaretçilerle etkileşim kurarken yaygındır.
            //Bunun nedeni, çöp toplayıcının yönetilmeyen nesneleri geri kazanmamadır.
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
