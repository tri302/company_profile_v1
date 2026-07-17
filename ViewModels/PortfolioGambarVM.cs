using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace company_profile
{
    public class PortfolioGambarVM
    {
        public int Id { get; set; }        
        public string? UrlGambar { get; set; }   

        public int IDPortfolio { get; set; }             
    }
}