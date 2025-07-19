namespace 記帳程式後端.Models
{
    public class ResponseData<T> : Response
    {
        public T Data { get; set; }

        public ResponseData(T Data)
        {
            this.Data = Data;
        }
        public ResponseData(int code, T Data)
        {
            this.Code = code;
            this.Data = Data;
        }
        public ResponseData(int code, String message)
        {
            this.Code = code;
            this.Message = message;
        }

    }
}
