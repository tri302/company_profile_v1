

namespace company_profile
{
    public interface ITestimonialRepo
    {
        //Get All Testimonials
        Task<IEnumerable<TestimonialVM>> GetAllTestimonialsAsync();

        //Update Testimonial
        Task<bool> UpdateTestimonialAsync(TestimonialVM testimonialVM);

        //Create Testimonial    
        Task<TestimonialVM> CreateTestimonialAsync(TestimonialVM testimonialVM);

        //Delete Testimonial
        Task<bool> DeleteTestimonialAsync(int idTestimonial);

        //Get Testimonial by Id
        Task<TestimonialVM> GetTestimonialByIdAsync(int idTestimonial);

        Task<TestimonialVM> GetPagedDataAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
    }
}

