﻿namespace LibApi.Requests;

public class RentRegister
{
    public int UserId { get; set; }
    public int BookCopyId { get; set; }
    public DateTime Until { get; set; }
}