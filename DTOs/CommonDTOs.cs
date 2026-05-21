namespace SipintarBeltim.DTOs;

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
}

public class PaginatedRequest
{
    public string? GeneralSearch { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class AuditTrailDto
{
    public string Id { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public object? OldValues { get; set; }
    public object? NewValues { get; set; }
    public string? ChangedColumns { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}

public class AuditTrailQuery
{
    public string? TableName { get; set; }
    public string? Action { get; set; }
    public string? UserId { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}
