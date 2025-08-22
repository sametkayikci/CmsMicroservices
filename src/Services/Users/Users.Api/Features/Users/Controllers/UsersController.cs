namespace Users.Api.Features.Users.Controllers;

/// <summary>
/// Kullanıcı yönetimi için CRUD uç noktalarını barındırır.  
/// Kullanıcı oluşturma, güncelleme, silme ve listeleme işlemlerini sağlar.  
/// Ayrıca kullanıcı silindiğinde ilişkili içerikler de temizlenir.
/// </summary>
[ApiController]
[Route("/users")]
public sealed class UsersController(IUserService service) : ControllerBase
{
    /// <summary>
    /// Sistemdeki tüm kullanıcıları listeler.
    /// </summary>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>Kullanıcıların listesi (200).</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));


    /// <summary>
    /// Belirtilen <paramref name="id"/> ile eşleşen kullanıcıyı getirir.
    /// </summary>
    /// <param name="id">Kullanıcı kimliği (Guid).</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>
    /// Kullanıcı bulunduysa <see cref="UserDto"/> (200).  
    /// Kullanıcı yoksa 404 Not Found.
    /// </returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => (await service.GetByIdAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();


    /// <summary>
    /// Yeni bir kullanıcı oluşturur.
    /// </summary>
    /// <param name="req">Yeni kullanıcı bilgilerini içeren istek modeli.</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>Oluşturulan <see cref="UserDto"/> nesnesi (201 Created).</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req, CancellationToken ct)
    {
        var dto = await service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }


    /// <summary>
    /// Belirtilen <paramref name="id"/> ile eşleşen kullanıcıyı günceller.
    /// </summary>
    /// <param name="id">Güncellenecek kullanıcının kimliği (Guid).</param>
    /// <param name="req">Güncelleme isteği modeli.</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>
    /// Güncellenmiş <see cref="UserDto"/> nesnesi (200).  
    /// Kullanıcı bulunmazsa 404 Not Found.
    /// </returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest req, CancellationToken ct)
        => (await service.UpdateAsync(id, req, ct)) is { } dto ? Ok(dto) : NotFound();


    /// <summary>
    /// Belirtilen <paramref name="id"/> ile eşleşen kullanıcıyı siler.  
    /// Kullanıcıya ait içerikler de otomatik olarak temizlenir.
    /// </summary>
    /// <param name="id">Silinecek kullanıcının kimliği (Guid).</param>
    /// <param name="ct">İstek iptal token’ı.</param>
    /// <returns>
    /// Başarılı olursa 204 No Content.  
    /// Kullanıcı bulunmazsa 404 Not Found.
    /// </returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}