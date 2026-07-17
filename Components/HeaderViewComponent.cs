using Microsoft.AspNetCore.Mvc;
using company_profile.ViewModels;
using System.Threading.Tasks;

namespace company_profile.Components
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IHeaderRepo _headerRepo;

        public HeaderViewComponent(IHeaderRepo headerRepo)
        {
            _headerRepo = headerRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listHeader = await _headerRepo.GetAllAsync();

            // Ambil data pertama yang IsShow == true, atau kalau tidak ada ambil data paling pertama
            var model = listHeader.FirstOrDefault(x => x.IsShow) ?? listHeader.FirstOrDefault();

            // Antisipasi jika database benar-benar kosong agar tidak null reference error di View
            if (model == null)
            {
                model = new HeaderVM();
            }
            return View(model);
        }
    }
}