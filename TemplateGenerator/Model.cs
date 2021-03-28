using System;
using System.Collections.Generic;


namespace TemplateGenerator
{
    public class User
    {
        public User()
        { }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Message
    {
        public Message()
        { }
        public string Author { get; set; }
        public DateTime SentAt { get; set; }
        public string Text { get; set; }

    }

    public class Room
    {
        public Room()
        { }
        public string Name { get; set; }
        public ICollection<Message> Messages { get; set; }
    }

    public class Model
    {
        public Model()
        { }
        public string Title { get; set; }
        public ICollection<User> Users { get; set; }

        public ICollection<Room> Rooms { get; set; }

    }
}
