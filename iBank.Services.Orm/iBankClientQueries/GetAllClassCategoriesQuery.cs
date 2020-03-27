using Domain.Interfaces;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllClassCategoriesQuery : IQuery<IList<ClassCategoryInformation>>
    {
        private string Agency { get; set; }
        private string LangCode { get; set; }

        private IClientQueryable _db;
        public GetAllClassCategoriesQuery(IClientQueryable db, string agency = "", string langCode = "EN")
        {
            Agency = agency.Trim().ToUpper();
            LangCode = langCode.Trim().ToUpper();
        }

        ~GetAllClassCategoriesQuery()
        {
            _db.Dispose();
        }

        public IList<ClassCategoryInformation> ExecuteQuery()
        {
            if (string.IsNullOrEmpty(Agency))
            {
                return _db.ClassCatMstr.Select(s => new ClassCategoryInformation
                                                        {
                                                            Agency = s.Agency,
                                                            Category = s.ClassCat,
                                                            Description = s.CatDescrip,
                                                            Heirarchy = s.Hierarchy
                                                        }).ToList();
            }
            else
            {
                return _db.ClassCatMstr.Where(x => x.Agency.ToUpper().Equals(Agency) && x.LangCode.ToUpper().Equals(LangCode))
                                        .Select(x => new ClassCategoryInformation
                                                            {
                                                                Agency = x.Agency,
                                                                Category = x.ClassCat,
                                                                Description = x.CatDescrip,
                                                                Heirarchy = x.Hierarchy
                                                            }).ToList();
            }
        }
    }
}
