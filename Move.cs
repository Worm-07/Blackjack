/*
 ***US11: Move – spelarens beslut levereras utifrån***
 */

/// Ett drag: köpa (ta ett kort till) eller stanna (avsluta sin tur).
/// Player frågar aldrig Console själv (US11) – beslutet levereras hit
/// som ett Move, t.ex. av ConsoleUi (US16–US18) eller av en
/// IPlayerStrategy (US22–US24).
public enum Move
{
    Hit,
    Stand
}
