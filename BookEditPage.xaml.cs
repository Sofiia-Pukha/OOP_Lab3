
namespace Lab3;

public partial class BookEditPage : ContentPage
{
    private Book _book; 
    private Action<Book> _onSave; 

   
    public BookEditPage(Book book, Action<Book> onSave)
    {
        InitializeComponent();
        _book = book;
        _onSave = onSave;

        
        TitleEntry.Text = book.Title;
        AuthorEntry.Text = book.Author;
        GenreEntry.Text = book.Genre;
        YearEntry.Text = book.Year > 0 ? book.Year.ToString() : "";
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
       
        _book.Title = TitleEntry.Text;
        _book.Author = AuthorEntry.Text;
        _book.Genre = GenreEntry.Text;

       
        if (int.TryParse(YearEntry.Text, out int year))
            _book.Year = year;
        else
            _book.Year = 0;

        _onSave?.Invoke(_book);

       
        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
       
        await Navigation.PopModalAsync();
    }
}