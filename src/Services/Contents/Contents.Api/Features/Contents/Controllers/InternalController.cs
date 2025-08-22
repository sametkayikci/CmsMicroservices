namespace Contents.Api.Features.Contents.Controllers;

/// <summary>
/// İçerik servisinde sadece iç kullanıma yönelik operasyonları barındırır.
/// Genellikle mikroservisler arası iletişim veya sistem yönetimi amaçlıdır.
/// </summary>
[ApiController]
[Route("/internal/contents")]
public sealed class InternalController(IContentService service, IConfiguration cfg) : ControllerBase
{
    /// <summary>
    /// Belirli bir kullanıcıya ait tüm içerikleri siler.
    /// Bu uç nokta sadece iç sistemler için tasarlanmıştır ve 
    /// "X-Internal-Key" header değeri ile doğrulama yapılır.
    /// </summary>
    /// <param name="userId">İçerikleri silinecek kullanıcının kimliği (Guid).</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>
    /// Başarılı olursa, etkilenen içerik sayısını içeren <c>200 OK</c> yanıtı.  
    /// Anahtar hatalıysa <c>401 Unauthorized</c>.  
    /// </returns>
    [HttpDelete("by-user/{userId:guid}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> DeleteByUser(Guid userId, CancellationToken ct)
    {
        var key = Request.Headers["X-Internal-Key"].ToString();
        if (key != cfg["Internal:Key"]) return Unauthorized();

        var affected = await service.DeleteByUserAsync(userId, ct);
        return Ok(new { affected });
    }
}