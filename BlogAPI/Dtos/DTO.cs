namespace BlogAPI.Dtos
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }   // enkel variant
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class CreatePostRequest
    {
        public int UserId { get; set; }      // från login
        public string Title { get; set; }
        public string Text { get; set; }
        public int CategoryId { get; set; }  // en av de du skapade (Fashion/Training/Health)
    }


    public class UpdatePostRequest
    {
        public int UserId { get; set; }      // den som försöker uppdatera
        public string Title { get; set; }
        public string Text { get; set; }
        public int CategoryId { get; set; }
    }


    public class CreateCommentRequest
    {
        public int UserId { get; set; }   // den inloggade användaren
        public string Text { get; set; }
    }

}
