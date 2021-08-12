using ProgrammersBlog.Shared.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Data.Abstract
{
    public interface IEntityRepository<T> where T:class,IEntity ,new()
        //Generic olan gönd class ın kontrolü örn. abstract class gönd zaman metodlar calışmaz IEntity ise Db de ki nesnelerimiz için ve de new yapılabilmeli
    {
        //Başka projelerde de kullanac. Generic Repository Async olab. olmayab. sadece projeye özgü
        Task<T> GetAsync(Expression<Func<T,bool>> predicate,params Expression<Func<T,object>>[] includeProperties); // var kullanıcı = repository.GetAsync(k=>k.id==15) 
        // params Expression<Func<T,object>>[] includeProperties 25 id'li bir makaleyi getirirken makale ile birlikte kullanıcıyı ve yorumlarıda include etmek istiyoruz
        // ThenInclude gibi...

        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate=null, params Expression<Func<T, object>>[] includeProperties);
        //Tüm kategorileri getirmek istiyoruz reporsitory.GetAllAsync(y=>y.ArticleID==1);

        Task<T> AddAsync(T entity); //ajax işlem. için generic yapmamız gerekli cunku geriye veri dönmesi gerekli ki json a cevirip view e gönd.
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        // var result = _userReporsitory.AnyAsync(u=>u.FirstName=="Ali"); var mı 
        Task<int> CountAsync(Expression<Func<T, bool>> predicate=null);

    
    }
}
