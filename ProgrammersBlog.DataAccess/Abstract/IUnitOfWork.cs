using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.DataAccess.Abstract
{
     public interface IUnitOfWork:IDisposable
    {
        //Unit of Work sayesinde tüm repositorileri aynı yerden yönet.
        //Örn. _articleRepository.AddAsync(article)
        //     _articleRepository.SaveAsync()  vb. tüm repoları yeniden yazmak yerine yerine
        // _unitofwork.Article.AddAsync(article) 
        //_unitofwork.User.AddAsync(user) 
        //
        IArticleRepository Articles { get; } //
        ICategoryRepository Categories { get; }
        ICommentRepository Comments { get; }
        IRoleRepository Roles { get; }
        IUserRepository Users { get; }
        Task<int> SaveAsync(); //Db ye kaydedilecek olan Transaction sayısı..
    }
}
