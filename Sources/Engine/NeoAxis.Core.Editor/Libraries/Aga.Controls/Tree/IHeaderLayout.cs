#if !DEPLOY
namespace Internal.Aga.Controls.Tree
{
  internal interface IHeaderLayout
  {
    int PreferredHeaderHeight
    {
      get;
      set;
    }

    void ClearCache();
  }
}

#endif