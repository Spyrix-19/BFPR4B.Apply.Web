using BFPR4B.Apply.Web._keenthemes.libs;

namespace BFPR4B.Apply.Web._keenthemes;

public interface IKTBootstrapBase
{
    void InitThemeMode();
    
    void InitThemeDirection();

    void InitLayout();
    
    void Init(IKTTheme theme);
}