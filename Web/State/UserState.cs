﻿namespace Web.State
{

    // Modelos simplificados de estado
    public class UserState
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

}
