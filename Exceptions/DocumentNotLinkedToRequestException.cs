namespace HRMS.Exceptions
{
    public class DocumentNotLinkedToRequestException : AppException
    {
        public DocumentNotLinkedToRequestException() : base("Document is not linked to the specified leave request.") { }
        public DocumentNotLinkedToRequestException(string message) : base(message) { }
        public DocumentNotLinkedToRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
