namespace Application.Records;

public record BlogDetailsRecord(Guid Id, string Title, string Body, string Location, string Reaction, List<string> Images);
