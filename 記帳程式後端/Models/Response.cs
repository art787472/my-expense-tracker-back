namespace 記帳程式後端.Models
{
    public class Response
    {
        public int Code { get; set; } = 200;
        public String Message { get; set; } = "Success";
        public bool IsSuccess { get; set; } = true;

        public Response()
        {

        }
        public Response(int Code, bool IsSuccess)
        {
            this.Code = Code;
            this.IsSuccess = IsSuccess;
            if (!IsSuccess)
                Message = "Fail";
        }
        public Response(int Code, bool IsSuccess, String Message)
        {
            this.Code = Code;
            this.IsSuccess = IsSuccess;
            this.Message = Message;
        }
        public Response(int Code, String Message)
        {
            this.Code = Code;
            this.Message = Message;
        }
        public Response(String Message)
        {
            this.Message = Message;
        }

    }
}
