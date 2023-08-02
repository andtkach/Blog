using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikeController : Controller
    {
        private readonly ILikeRepository likeRepository;

        public LikeController(ILikeRepository likeRepository)
        {
            this.likeRepository = likeRepository;
        }

        [Route("Add")]
        [HttpPost]
        public async Task<IActionResult> AddLike([FromBody] AddLikeRequest addLikeRequest)
        {
            await likeRepository.AddLikeForBlog(addLikeRequest.ArticleId,
                addLikeRequest.UserId);

            return Ok();
        }


        [HttpGet]
        [Route("{articleId:Guid}/totalLikes")]
        public async Task<IActionResult> GetTotalLikes([FromRoute] Guid articleId)
        {
            var totalLikes = await likeRepository.GetTotalLikesForBlog(articleId);

            return Ok(totalLikes);
        }
    }
}
