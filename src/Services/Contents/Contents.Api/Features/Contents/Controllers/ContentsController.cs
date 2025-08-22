namespace Contents.Api.Features.Contents.Controllers;

/// <summary>
/// İçerik CRUD uç noktalarını barındırır.
/// Kullanıcı doğrulaması ve mikroservisler arası senaryoları da içerir.
/// </summary>
[ApiController]
[Route("/contents")]
public sealed class ContentsController(IContentService service) : ControllerBase
{
    /// <summary>
    /// Tüm içerikleri getirir.
    /// </summary>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>Tüm içeriklerin listesi.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ContentDto>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));


    /// <summary>
    /// Belirtilen <paramref name="id"/> değerine göre içeriği getirir.
    /// </summary>
    /// <param name="id">İçerik kimliği (Guid).</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>İçerik bulunduysa <see cref="ContentDto"/>; aksi halde 404.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContentDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => (await service.GetByIdAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();


    /// <summary>
    /// Yeni içerik ekler.
    /// İçerik oluşturulmadan önce User Service üzerinden kullanıcı doğrulaması yapılır.
    /// </summary>
    /// <param name="req">Oluşturulacak içeriğin bilgileri.</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>Oluşturulan <see cref="ContentDto"/> nesnesi.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ContentDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateContentRequest req, CancellationToken ct)
    {
        var dto = await service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto!.Id }, dto);
    }


    /// <summary>
    /// Belirtilen <paramref name="id"/> değerine sahip içeriği günceller.
    /// </summary>
    /// <param name="id">Güncellenecek içeriğin kimliği (Guid).</param>
    /// <param name="req">Güncelleme isteği modeli.</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>Güncellenmiş <see cref="ContentDto"/> nesnesi veya 404.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContentDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContentRequest req, CancellationToken ct)
        => (await service.UpdateAsync(id, req, ct)) is { } dto ? Ok(dto) : NotFound();


    /// <summary>
    /// Belirtilen <paramref name="id"/> değerine sahip içeriği siler.
    /// </summary>
    /// <param name="id">Silinecek içeriğin kimliği (Guid).</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>NoContent (204) veya 404.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();


    /// <summary>
    /// DEMO senaryosu: İçerik mikroservisi üzerinden User Service'e çağrı yaparak
    /// kullanıcının bilgilerini günceller.
    /// </summary>
    /// <param name="userId">Güncellenecek kullanıcının kimliği (Guid).</param>
    /// <param name="req">Kullanıcı güncelleme isteği modeli.</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>NoContent (204).</returns>
    [HttpPost("users/{userId:guid}/update-from-content")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateUserFromContent(Guid userId, [FromBody] UpdateUserRequest req,
        CancellationToken ct)
    {
        await service.UpdateUserViaUsersApiAsync(userId, req, ct);
        return NoContent();
    }
}