﻿using System.ComponentModel.DataAnnotations.Schema;

namespace LibApi.Model;

public class BooksGenre : DbEntity
{
    [ForeignKey("Book")]
    public int BookId { get; set; }
    public Book Book { get; set; }
    
    [ForeignKey("Genre")]
    public int GenreId { get; set; }
    public Genre Genre { get; set; }
}