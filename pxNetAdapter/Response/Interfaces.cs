namespace pxNetAdapter.Response
{
    public interface IResponse
    {
		ResponseTypeEnum Qualifier { get; }
        string RequestId { get; }
		IError Error { get; }
		IResponseData Data { get; set; }
    }

    public interface IError
    {
        string Code { get; }
        string Message { get; }
    }

    public interface IResponseData
    {
    }
}
