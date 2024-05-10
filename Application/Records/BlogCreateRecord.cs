namespace Application.Records;

public record BlogCreateRecord(string Title, string Body, string Location, string Reaction, List<string>? Images);
