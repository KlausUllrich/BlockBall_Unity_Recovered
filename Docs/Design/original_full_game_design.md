BLOCKBALL EVOLUTION

Game Design Document

V 0.9

# Game Design

## Spielkonzept

#### Basic Game Concept

„BlockBall evolution“ ist ein 1 Player Knobelspiel, bei dem man den Exit eines Levels erreichen muss. Der Spieler rollt dabei in einer drei-dimensionalen Welt auf einer Plattform aus Blöcken. Der Spieler muss versuchen nicht von der Plattform zu fallen. Er kann jedoch an bestimmten Stellen die Gravitationsrichtung ändern und so neue Stellen eines Levels erreichen.

#### Target Group

Das Spiel soll keine Altersbeschränkung erhalten. Durch den geistigen Anspruch richtet es sich jedoch eher an knobelwillige Spieler. Das dynamische Gravitationssystem erfordert zudem Geschicklichkeit und Disziplin. Das Spiel zielt daher eher auf erfahrene und ältere Spieler ab. Eine langsame Einführung soll es jedoch auch unerfahrenen Spielern ermöglichen sich in das Spiel hineinzuarbeiten.

#### Basic Gameplay

Um das Ende eines Levels erreichen zu können, müssen Hindernisse, wie getrennte Plattformen und verschlossene Türen umgangen werden. Dabei muss der Spieler geschickt mit der Trägheit des Balls umgehen, um nicht von der Plattform zu fallen. Wird eine Boden-Kante an bestimmten, abgerundeten Stellen überschritten, ändert sich die Gravitationsrichtung für den Ball, so dass die vorherige Seite eines Levels nun für diesen zum neuen Boden wird. Die Puzzelaufgaben werden so anspruchsvoller, da „um die Ecke“ gerollt werden kann.

#### Key-Features

- Dynamisches Gravitationssystem und Perspektivenwechsel
- 15 Levels
- Leveleditor

#### Expanded Gameplay Description

Das Spiel bietet eine neuartige Kombination von Spielelementen: Zum einen ist es ein Geschicklichkeitsspiel, bei dem gekonnt ein Ball auf einer Plattform gesteuert werden muss. Zum anderen ist es aber auch ein Knobelspiel, bei dem durch surreale physikalische Regeln und verschiedenen Spielelementen komplexe Logik-Rätzel zu lösen sind.

Die Kamera-Ansicht ist direkt hinter dem Ball von leicht schräg oben. Dreht der Spieler seinen Ball so dreht sich die Kamera leicht verzögert hinterher. Der Spieler kann sich mit der Maus frei im Level umsehen. Die Camera schwenkt dabei um den Ball immer mit Blickrichtung in Richtung Ball. Bei dem Rollen „über die Ecke“ bleibt die Kamera stets hinter dem Ball. Behindern Blöcke die Sicht, so werden diese ausgeblendet. Der Spieler kann zudem hinaus zoomen um einen besseren Überblick zu erhalten.

Die Herausforderung des Geschicklichkeitsteils liegt hauptsächlich in der gekonnten Steuerung des Balls um ein hinunterfallen von der Spielplattform zu verhindern.

Neben flachen Ebenen gibt es dabei auch Schrägen, die den Ball entweder in der Bahn halten oder ihn von ihr abdrängen. Rollt der Ball längere Schrägen hinunter wird der Ball schneller, rollt er Schrägen hinauf, langsamer. Der Ball verhält sich physikalisch korrekt.



Level Auswahlbildschirm Leveleditor



Ein Level

Der Knobelteil ist die Kernherausforderung des Spiels. Die Diamanten und der Exit eines Levels sollen nicht einfach zu erreichen sein. Der Spieler muss je nach Level andere Arten von Herausforderungen meistern. Die einfachsten Rätzel sind dabei noch das Aufsammeln von Schlüsseln, um durch entsprechende Türen rollen zu können. Vor allem steht dem Spieler jedoch die Beschaffenheit des Levels im Weg. Er muss sein Vorgehen daher mit Bedacht wählen.

Eine große Besonderheit ist das dynamische Gravitationssystem. So ist es möglich, an bestimmten Stellen über eine Kante hinaus zurollen. Die Gravitationsrichtung passt sich dort dem Ball an, so dass er „auf die Seite“ der Plattform rollen kann. Durch die geänderte Gravitationsrichtung ist die ursprüngliche Seite aber nun der neue Boden des Levels. Der Ball kann auf dieser „neuen“ Ebene normal gesteuert werden. So ist es dem Spieler möglich, völlig neue Bereiche des Levels zu erreichen.

Dieses „um die Ecke rollen“ kann aber nur an bestimmten, speziell abgerundeten

Stellen erfolgen. Rollt der Spieler über eine normale Kante, so zieht ihn die Schwerkraft nach „unten“. Das kann durchaus gewollt sein, um so eine „darunter“ liegende Ebene zu erreichen. Ist jedoch keine Ebene unterhalb oder ist diese zu weit entfernt, so würde der Ball ins Leere fallen und der Spieler respawned bei dem letzten von ihm aufgesammelten Diamanten.



Gravityswitches

Der Spieler kann an solchen abgerundeten Stellen jedoch nicht nur nach „unten“ sondern auch nach „oben“ rollen. Die runden Stellen bieten generell einen Übergang auf eine andere, um 90° gedrehte, Ebene. Durch mehrmaliges Nutzen solcher Übergänge kann der Spieler z.B. auch zu seiner ursprünglichen Richtung auf dem Kopf stehen oder um die Level kanten herum wieder zu seiner alten Position zurückkehren. Die Gravitationsrichtung ändert sich aber stets nur für seinen Ball.

Das Spiel kann über Tastatur, Gamepad und Maus (ggf. auch 3D-Maus) gesteuert werden. Der Ball kann nach vorne, hinten, links und rechts bewegt werden. Der Ball kann zudem springen. Die Kamera kann per Maus um den Ball gelenkt werden. Der Spieler kann sich jedoch per Tastendruck auch umsehen, um so den Level besser zu erkunden.

Grundsätzlich kann sich der Spieler soviel Zeit für einen Level nehmen wie er will oder braucht. Die Herausforderung liegt jedoch darin so viel Diamanten so schnell wie möglich aufzusammeln. Passend dazu soll die Musik treibend und zum Level thematisch passend sein. Das Spiel soll ein Puzzlespiel mit dem Flair eines C64 Titels, aber in technisch modernem Gewand werden. Die Levels sollen kombinatorisches Denken, räumliches Vorstellungsvermögen und Geschicklichkeit erfordern.

Um in das Spiel eingeführt zu werden sind in den ersten Levels Infofelder eingebaut.

Rollt der Ball über diese, so erscheint ein Informationstext. Es gibt 15 Levels im Spiel. Ist ein Level einmal geschafft, so kann der Spieler immer wieder ab diesem Level weiterspielen.

Das Spiel enthält zu dem einen Level Editor, mit dem Spieler schnell und frei ihre eigenen Levels erstellen können. Der Level-Editor ist in seinem Funktionsumfang klar abgegrenzt und soll von der gleichen Zielgruppe bedient werden können.

#### Target Plattform / Technologie

Das Spiel wird für PC entwickelt. Als Entwicklungsumgebung wird Visual Studio und die OGRE Engine verwendet. Für die Physik wird die PhysX-Engine verwendet.

#### Sprache (verschoben, vorerst nur English)

Das Spiel bietet die Möglichkeit die Sprache zu wechseln. Derzeit sind Englisch und Deutsch implementiert. Die Sprache wird mit dem jeweiligen Profil gespeichert. Im Installer kann aber eine Default-Language gewählt werden.

#### Grafik

Die Grafik wird sich in Blockball Evolution im Gegensatz zu Blockball V1.0 deutlich unterscheiden. Es wird ein Stiel angestrebt, der sich an Portal und Audio Surf (weiße Level) orientiert.

 

Die Farben des Spiels bestehen aus weiß/grauen (mit „Occlusion-Look“) sowie pastellfarbenen Blöcken. Der Look ist futuristisch modern. Der Stiel stellt Erkennbarkeit und Funktionalität vor Buntheit und Abwechslung. Der Look ist minimalistisch aber nicht langweilig.

Um die Spielererfahrung möglichst angenehm zu halten, sollen leichte positive Assoziationen zu realen Objekten bestehen. Das Hintergrundsetting soll grob an eine Stadt erinnern.



Die vorherrschende Farbe der Levels ist weiß. Die weiteren Block-Farben dienen der Spielerführung oder der Verzierung. Eine Farbpalette eines Texturesets bietet daher sowohl deutlich saturierte als auch blasse Farben.

Das Menü ist klar und präzise. Es wird ein hoher Kontrast verwendet. Schrift und Formen sind modern. Das Menü soll aufgeräumt und übersichtlich wirken.





Die Blöcke und Gegenstände besitzen eine hohe Symbolik, so dass sie weitgehend selbsterklärend sind. Es wird wie in Blockball V1.0 Wege und Signalfarben geben. Diese sind jedoch nicht mehr so grell. Signalfarben werden z.B. verwendet für Gravityswitches, Kanten, Wege, bewegliche Objekte, Ziel, Schlüssel und Tore. Zu den Signalfarben werden Partikelleffekte, wie Trials oder Rauch eingesetzt.

Es wird ein Prototyp erstellt, in dem der Level in eine Szene eingebaut ist. Es handelt sich dabei um eine komplett in Max/Maya erstellte Szene, welche die Orientierung erleichtert und den Level interessanter macht.



Die Szene ist von der Skalierung wesentlich größer als der Level. Selbst bei maximaler Entfernung der Kamera zum Ball kann die Kamera niemals mit der Szene in Konflikt treten. Die Blöcke wirken daher im Verhältnis zu der Szene sehr klein. Die Hintergrundgrafiken bestehen aus einer Skybox mit hochauflösenden Texturen, die zu der jeweiligen Szene passen.

Es gibt 6 grafisch unterschiedliche Hintergrund-Settings. Diese bestehen aus einer

Plane als Boden. Diese hat eine Grid in deutlich höherem Maßstab als die

Blockbreiten. Die Bodentextur ist in blasser homogener Farbe gehalten. Die Gridflächen sind gekrisselt ähnlich wie die Bodenplatten bei Portal. Der Farbton variiert je Schwierigkeit.

Die Bodenplatten haben unterschiedliche Höhen und erzeugen so eine unruhige Oberfläche. Alternativ zu den Bodenblockerhebungen sind vereinzelt wesentlich größere Objekte da. Diese dienen als visuelle Bezugspunkte zur Orientierung. Um diesen Effekt zu unterstützen können diese sich farblich abheben und über das Höhenniveau des eigentlichen Levels hinausragen.

Ein weißer Nebel blendet Boden und Objekte in der Distanz sanft aus. Im Hintergrund wird eine weiße Skymap verwendet, die „oben“ leicht ins farbliche verläuft. Der „Himmel“ ist leicht strukturiert.

Grundsätzlich variieren alle 6 Settings in ihrem Farbthema.

#### Sound

Die Soundeffekte werden vorerst aus BB v1.0 übernommen bis jemand gefunden wurde der neue Soundeffekte für uns erstellt.

Der musikalische Stiel soll gegenüber BB V1.0 noch elektronischer werden. Es werden Synthesizer Stücke verwendet, die auch mehrmals und wiederholt gehört werden können. Das Musikstück ist dabei einige Minuten lang und abwechslungsreich.

Es wird nach möglichst kostenlosen und freien Quellen gesucht.

Aufgrund der Rechteproblematik wird auf Music verzichtet.

## Detailbeschreibung Features

#### Respawning

Wird der Ball eines Spielers durch einen Fall-off oder anderweitig zerstört, so spawned dieser unmittelbar bei der Position des zuletzt aufgesammelten Diamanten wieder. Wurde noch kein Diamant aufgesammelt so wird am Startpunkt respawned. Die Blickrichtung der Kamera entspricht dabei genau der Stellung, wie sich die Kamera befand, als der Diamant aufgesammelt wurde. Durch das Respawning erhält der Spieler außer der Wartezeit für den Kameraflug zurück keine weiteren Nachteile.

Die Kamera fliegt dabei von ihrer Position zum Zeitpunkt des „Sterbens“ zurück in die

Position und Blickrichtung, an der sie sich zum Zeitpunkt des Aufsammelns des Diamanten befand. Zudem zoomt die Kamera während des Flugs leicht raus, um einen besseren Überblick über den Respawnpunkt zu bekommen.

#### Minimap (nicht notwendig)

Variante 1 zoomt die Kamera weit hinaus, solange eine Taste gedrückt ist (must have Variante). Zudem wird eine Art Hud eingeblendet, dass alle Schlüssel, Tore und das Ziel als Symbole auf dem Bildschirm vor allen anderen grafischen Elementen (außer den schwarzen Balken) anzeigt. Die Symbole für Schlüssel und Schlösser haben die entsprechende Farbe zu ihrem Ziel. Schlüssel haben ein anderes Symbol als Türen.



Die Symbole für die Elemente sind:

Tür: ???

Schlüssel: ???

Ziel: ???

Nice to have:

Variante 2 zoomt in den Ball hinein und macht diesen transparent. In dem Ball werden alle wesentlichen Objekte symbolisch dargestellt. Der Ball selbst wird als Miniatur in der Ballmitte platziert. Um ihn herum „schweben“ in proportional richtiger Entfernung alle Schlüssel, Tore und der Exit. Blöcke und sonstige Objekte werden nicht dargestellt. Der Level wird jedoch weiterhin im Hintergrund außerhalb des Balls dargestellt. Der Spieler kann mit der Maus die Kamera verändern und sich so grob orientieren in welcher Richtung er zu suchen hat. Ein Kombination von beiden Varianten wird geprüft.

#### Leveldateien

Die Leveldateien bestehen aus einem verschlüsselten ZIP-File. Darin sind alle notwendige Dateien enthalten. Levels werden so austauschbar, da sie verschlüsselt sind und alle relevanten Daten in einer Datei vereinen. Auf die Highscores kann durch eine getrennte Datei wenigstens schneller zugegriffen werden. Die Leveldateien enden auf die Endung „.level“.

s.a. Technisches Design Dokument

## Design Regeln

#### Springen

Der Ball springt immer gleich hoch. Die Sprunghöhe beträgt ca. 6 Bixel. Die Sprungweite aus dem Stand beträgt ca. 6 Bixel. Die Sprungweite bei voller „Fahrt“ beträgt ca. 12 Bixel.

Die Sprungtaste kann kurz vor dem und kurz nach dem Aufkommens des Balls gedrückt werden, um den Ball zum springen zu bringen. So muss vermieden werden, dass die Sprungtaste gedrückt wird, aber kein Sprung erfolgt, obwohl der Spieler der Meinung ist, dass der Ball hätte springen müssen. Die Regel lautet wie folgt: TTastendruck – TBodenkontakt < TDifferenz oder

TBodenkontakt – TTastendruck < TDifferenz Sprung

Hält der Spieler die Sprungtaste gedrückt, so springt der Ball unaufhörlich.

Wird die Vorwärts- und die Seitwärts-Rollen-Taste gleichzeitig gedrückt, so rollt der Ball genauso schnell, als wenn nur eine Rollen-Taste gedrückt ist.

#### Fallen

Die maximale Fallhöhe beträgt 32,5 Bixel (also gerade so 4 Blöcke). Die Fallhöhe ist unabhängig von der Airtime (in BB v1.0).

Ab einer Fallhöhe von 25 Bixeln gilt der Fall als ein harter Fall. Es wird ein eigener Sound FX abgespielt und ein dezenter Partikeleffekt wird am Ball gezeigt (kleines Staubwölkchen oder so), wenn der Ball aufschlägt.

#### Ziel erreichen

Das eigentliche Ziel besteht nicht aus dem Model, sondern aus der unsichtbaren

Kollisionsbox im Zielmodell. Berührt der Spieler dieses Ziel, so ändert sich die Gravitationsrichtung in Richtung des Mittelpunkts des Zielmodels und die Kamera rotiert ums Ziel. Der Spieler hat damit den Level geschafft.

#### Bonus Levels

Sammelt der Spieler einen großen Diamanten ein erhöht sich sein Gesamt-Konto an großen Diamanten für dieses Profil. Immer wenn der Spieler 5 neue Diamanten eingesammelt hat, ist der nächste Level in seinem Campain ein Bonus-Level. Der Bonus-Level zählt als regulärer Level und wird auch im Start-Menü angezeigt.

Wird ein Bonus Level jedoch abgebrochen, so wird im Hauptmenü der nächste Level im Campain angezeigt und ist somit auch der nächste Level. Freigeschaltete Bonus Level können jederzeit im „play campain“-Menu angewählt werden.

## GUI

#### InGame GUI

Das InGame GUI passt zu dem modernen minimalistischem Stiel der Level-Grafik. Es werden nur die wesentlichen Informationen dargestellt. Die verwendete Schrift hat einen schwarzen Konturrahmen, um sich stärker vom Hintergrund abzuheben.

Folgende Elemente werden in der GUI dargestellt:

#### Leveltimer

Der Leveltimer zeigt die Zeit in Minuten und Sekunden (Format: ##:##) seit Spielbeginn des Levels an. Der Timer startet erst, wenn die Kamera nach der einführenden Kamerafahrt beim Ball angekommen ist. Wird ein Menü aufgerufen, wird das Spiel pausiert und der Timer läuft nicht weiter. Ist mehr als 60 Minuten Zeit verstrichen, bleibt der Timer bei 60:00 stehen.

#### Time-Balken

Der Time-Balken zeigt die verschiedenen Time Frames für jede Bonuszeit an. Der Balken wird mit fortschreitender Zeit immer kleiner. Die Abgrenzungen je Time Frame sind klar zu erkennen. Es wird zudem ein kleiner Timer angezeigt, der die Restzeit für diesen Time Frame angibt, wenn weniger als 15 Sekunden Restzeit bis zum nächsten Timeframe übrig sind. Die Zeit wird in Sekunden und Zentelsekunden angegeben. Somit ist ständig Bewegung bei der Timebar bevor ein Zeitwechsel stattfindet. Der Timer wandert mit dem Balken nach unten. Bei Erreichen eines neuen Time Frames macht sich der Balken kurz visuell und akustisch bemerkbar. Der Timer bleibt dabei noch kurz bei der Position des letzten Time Frames stehen und die Nullen blinken schnell. Ist der gesamte Balken abgelaufen, verschwindet auch der Timer.

Ist die Zeit für die Mad Genius Zeit so knapp eingestellt, dass der Timer in Höhe der Dimantenanzeige ist wird dieser einfach hinter der Dimantenanzeige dargestellt. Ist der Timer in Höhe des oberen Balkens wird dieser weiß dargestellt. Ist der Timer am unteren Ende der Timeframebar, so wird der Timer ebenfalls weiß dargestellt.



Jedes Time Frame hat eine eigene Farbe. Die Balkenfarbe ändert sich komplett auf die jeweils aktuelle Farbe.

#### Diamanten Counter

Der Diamanten Counter zeigt alle eingesammelten Diamanten im Vergleich zu den im Level vorhandenen an.

Wird ein großer Diamant eigesammelt wird gut sichtbar ein Infotext „Secret Diamond found!“ eingeblendet. Zudem wird die Anzahl der bisherigen eingesammelten großen Diamanten mit diesem Profil angezeigt („3 secret diamonds collected so far. 2 more needed for bonus level“). Die Einblendung wird durch einen Sound unterstützt.

Wurde ein Bonuslevel freigespielt erscheint nach der obigen Einblendung anschließen eine neue Meldung „Bonus Level unlocked!!“. Der nächste Level ist somit ein Bonuslevel.

#### Schlüssel

Die Schlüssel werden oben links angezeigt. Wenn der Spieler das Level startet wird er die Anzahl an möglichen Schlüssel im GUI in Grau sehen.

Es gibt vier Schlüssel und entsprechend vier Tore. Wird ein Tor mit dem Schlüssel geöffnet bleibt der Schlüssel jedoch weiterhin am Ball. D.h. wurde einmal ein Schlüssel einer Farbe aufgenommen, so kann der Spieler damit alle entsprechenden Türen öffnen.

#### On-Screen Hilfe (verschoben)

Per Tastendruck können Erklärungen für

1. das Interface (Taste „G“)
2. die Tastaturbelegung (Taste „Tab“) angezeigt werden. Dazu wird eine den gesamten Bildschirm füllende Grafik eingeblendet, in der die Elemente erklärt werden. Der Hintergrund ist weiß/transparent. Die Hilfe wird nur so lange angezeigt, wie die Taste gedrückt wird. Das Spiel befindet sich so lange im Pausenmodus.

Bei der Hilfe zu den Tasten werden nur die Steuerungstasten und deren Belegung in einer Tabelle angezeigt. Die Hilfe für das Interface hat Pfeile und einen erklärenden Text. Die Sprache ist abhängig vom Profil.

#### Infobar

Im unteren schwarzen Balken werden verschiedene Informationen bedarfsgerecht eingeblendet. Der Balken wird somit als exklusiver Platz für diese Informationen reserviert. Die Informationen verdecken nicht den Sichtbereich im Spiel.

**! Nachfolgendes wird verschoben !**

Die Infobar analysiert das Spiel des Spielers und gibt möglichst kontextsensitiv Feedback zum Spiel. Hat der Spieler Probleme mit der Steuerung, so wird sie ihm erklärt, ist die Spielperformance schlecht, erhält er Hinweise, wie er diese Verbesser kann, macht der Spieler gut Spielzüge, so sagt ihm das Programm, dass er gut spielt.

Um die Informationen für den Spiel leicht wahrnehmbar zu machen haben sie einen hohen Kontrast (schwarz/weiß), je Informationstyp ein eigenes Pictogramm und der Informationstext wird animiert eingeblendet.

Folgende Informationen werden angezeigt:

Levelinformationen - zu Beginn des Levels

Informationstexte - beim Berühren von Infofeldern

Hilfetexte - Kontextsensitiv bei Bedarf

Hinweise - im Pausenmodus

Es können 45 Zeichen pro Zeile und max. zwei Zeilen dargestellt werden (genaue zahlen sind aber noch zu testen). Wird nur eine Zeile dargestellt, so ist diese zentriert. Die Animation blendet (sehr schnell) alle Zeichen der Reihe nach ein.

Die dargestellten Informationen werden für eine fixe Zeit dargestellt und verschwinden dann (animiert) wieder von allein. Nur die Texte der Infofelder werden ggf. länger dargestellt, wenn der Spieler noch im Inforfeld steht.



#### Levelinformationen

- Schlüsselinformationen
- Highscore

#### Informationstexte

Infotexte können im Editor optional im Level eingebaut werden und sollen dem Spieler Hinweise für diesen speziellen Level geben. Sie sollen jedoch keine Steuerungshinweise mehr wie in Blockball V1.0 geben.

Zusätzlich gibt es im Editor auch unsichtbare Infotexte. Diese können ebenfalls einen Infotext im unteren Balken ausgeben. Sie stellen jedoch keine Bedarfsinformation da, sondern sollen den Spieler zu Begin mit Hinweisen, wie „Well Done!“ oder „Secret found!“ motivieren.

Als Pictogramm wird das Infofeldsymbol verwendet.

#### Hilfetexte (verschoben)

Um den Spieler nicht mit zuviel Informationen zu erschlagen, werden in den ersten Levels bei Bedarf oder abwechselnd grafisch unterstütze Steuerungshinweise angezeigt. Dazu werden der jeweilige Effekt als Pictogramm sowie ein kurzer Text gezeigt. Folgende Hinweise werden gezeigt:

- 1.WASD Tastatursteuerung
- 2.Springen
- 3.Maussteuerung
- 4.Mausinvertierung im Optionsmenü
- 5.Grafikeinstellungen, wenn wenig FPS
- 6.Minimap
- 7.Taste alle Keys
- 8.Taste Info Interface

Manche Elemente werden nur bei Auslösen eines internen Triggers abgespielt (z.B. Spieler drückt 5 Sek. keine Taste, FPS < 20), andere befinden sich im Queue und werden alle 5 Sekunden für 5 Sekunden der Reihe nach eingeblendet.





#### Level Design Styleguide

## Levelarten(verschoben)

Es gibt 4 verschiedene Arten an Levels:

Tutorial Level

Special Theme Level

Knobel Level

Racing Level

Bonus Level

Der Aufbau der Levels ist so gestaltet, dass regelmäßig neue Elemente vorgestellt werden. Somit wird sichergestellt, dass es immer wieder neues zu Endecken gilt und die Puzzles abwechslungsreich sind. Zudem wechseln sich Knobellastige Level mit Racing Leveln ab. Der Spieler wird somit nach harter Denkarbeit wieder mit etwas Aktion belohnt.

Die Level unterteilen sich in Basic, Advanced und Hard Levels. Diese werden bei der Levelauswahl im Menü getrennt dargestellt. Tutorial Level werden nicht als solche benannt, sondern gehören zur Kategorie Basic. Für einen neuen Spieler ist nur der erste Basic Level freigeschalten. Wird ein Level geschafft, so wird der jeweils darauf folgende Level freigeschalten. Ein einmal freigeschaltener Level bleibt auch freigeschalten.

Wird der Campaign gespielt, werden automatisch alle Level nach ihrer Reihenfolge nach gespielt. Die Levels schalten sich so automatisch frei. Einzelne Level können nur über das Menü „Play Single Level“ ausgewählt werden. Beim Beenden eines Levels hat der Spieler aber immer die Möglichkeit auszuwählen, ob er wieder ins Menü oder einfach am nächsten Level weiterspielen möchte.

#### Tutorial Level

Tutorial Level sind sehr einfach zu Lösen und sollen dem Spieler den Einstieg ins Spiel ermöglichen. Der Aufbau ist linear und der Spieler hat keine Möglichkeit sich zu verfahren oder abzukürzen. Zudem wird der Spieler stark durch Farben geführt. Die Pro-Zeit ist kurz und beträgt max. 1 Minute. Die Silberzeit ist jedoch mit über 5 Minuten sehr hoch angesetzt, so dass ein unerfahrener Spieler viel Zeit hat sich an das Spiel und die Steuerung zu gewöhnen. Die Goldzeit ist ebenfalls großzügiger als in späteren Levels eingestellt.

Die Besonderheit der Tutorial Level sind die Infoboxen. In diesen werden zunächst die Steuerung, Optionen zur Steuerung, das Interface, die Spielmechnik und später auch Tips und Tricks erklärt. Pro Tutorial Level sollten jedoch nicht mehr als 4 Infoboxen enthalten sein.

Level 1: - Steuerung

- Ziel finden
- Optionsmenü: Maus invertieren (und Optionsmenü allgemein)
- Diamanten aufsammeln

Level 2: - Gravity Switches

- Fall-off
- Jumping
- (entfernt: Doppel-Sprung)

Level 3: - Schlüssel

- Tür
- versteckte Diamanten

Level 4: - Minimap

- Tipps zur Orientierung (rausscrollen, Farben, drehen wenn Minimap an)
- Pfeile

Level 5: - Zeit und Punkte

- Bonuspunkte
- Medallien
- Highscore

Werden in späteren Levels neue Elemente eingeführt, so können für diese ebenfalls wieder Infoboxen zur Erklärung hinzugefügt werden. Auch besonders schwere Stellen können durch Infoboxen erleichtert werden.

#### Special Theme Level

Special Theme Level sind zum einen Tutorial Level, da sie Infoboxen enthalten und neue Levelelemente vorstellen. Zum anderen haben sie aber auch jeweils ein bestimmtes Setting, das sich an einem Spiele-Klassiker orientiert. Die Special Theme Level sind jeder 5. Level. Somit kann der Spieler sich bereits darauf freuen, immer dann neue Elemente vorgestellt zu bekommen.

Level 10 - Frogger

Level 15 - (Schalter) “Verrücktes Labyrinth”

Level 20 - Donkey Kong

Level 25 - Sokoban

Level 30 - Finales Level

_L10 - Frogger:_

In diesem Level werden bewegliche Blöcke eingeführt. Das Frogger Theme gibt es in zwei Varianten:

1. Der Spieler muss mit seinem Ball über bewegliche Blöcke über einen Abgrund springen. Die Blöcke bestehen dabei aus 1-Block-breiten Ketten, die sich immer nur in eine Richtung bewegen. Fällt der Spieler von den Blöcken, so fällt er aus dem Level.
2. Der Spieler rollt auf einer Plattform und muss sich bewegenden Blockketten ausweichen. An den Enden der Blockketten sind Stacheln. Kollidiert der Spieler mit den Stacheln ist sein Ball kaputt.

Sich bewegende Blöcke haben immer die gleiche Farbe in allen Leveln (z.B. Orange). Orange ist somit Signalfarbe und heißt entweder, dass sich der Block bereits automatisch bewegt oder dass er zumindest durch Schalter zum bewegen gebracht werden kann.

_L 15 - (Schalter):_

Hier werden Schalterrätzel eingeführt. Mit einem Schalter kann der Spieler gezielt Blöcke bewegen. So kann z.B. eine Lücke geschlossen werden, ein Fahrstuhl aktiviert oder ein ganzes Levelsegment gedreht werden.

_L 20 - Donkey Kong_

In diesem Level werden weitere physikalische Objekte eingeführt. In Level 20 muss der Spieler einen Level „hoch“ rollen, während ihm ständig Bälle entgegenrollen. Die Bälle bringen den Spieler von seiner Bahn ab.

Folgende Elemente kommen vor:

| Ballemitter | \- Je Zeitintervall fallen Bälle aus dem Emitter. Diese Bälle unterscheiden sich farblich deutlich vom eigenen Ball. |
| --- | --- |
| Kisten | \- 1 Kiste kann durch den Ball verschoben werden, 2 Kisten auf einmal können nicht verschoben werden und halten auch Fässer auf. Kisten sind schwer genug einen Schalter einzudrücken.<br><br>Kisten können über Gravityswitches verschoben werden. |
| Bälle | \- Bälle sind ungefähr so groß wie der eigene Ball, aber deutlich schwerer. |
| Beschleuniger | \- Rollt ein Ball vor dieses Objekt, wird der Ball nach vorne geschossen. In der Animation wird eine Art Metallstift nach vorne gestoßen. Danach muss sich der Metallstift langsam zurück bewegen. Erst nach dieser „Nachladezeit“ von 2 Sekunden kann er wieder „schießen“. |

L 25 – Sokoban

Der Spieler muss ab diesen Levels kombinatorische Rätzel lösen. Diese lehnen sich zum Teil an das Spiel Sokoban an, bei dem Kisten in einer bestimmten Reihenfolge verschoben werden müssen. Der Spieler kann sich jedoch nie seinen Weg so verbauen, dass er das Ziel nicht mehr erreichen kann. Weitere kombinatorische Rätzel sind das Bauen von Brücken oder Treppen, die dem Spieler das Weiterkommen ermöglichen oder Bonus-Diamanten geben. Die Rätzel sind jedoch keine wirklichen „harten Nüsse“, sondern sollen einfach etwas Abwechslung bieten.

L 30 – Finales Level

Das letzte Level ist das schwerste Level des Spiels und soll alle bisherigen Erfahrungen und das Können des Spielers noch einmal kombiniert abfragen.

#### Racing Levels

Racing Levels sind sehr schnell und erfordern kein Lösen von Knobelaufgaben. Auch der Geschicklichkeitsanteil ist stark eingeschränkt auf das Optimieren der Wegfindung oder der „Ideallinie“. Der Spieler soll durch diese Level mit etwas Aktion belohnt werden. Zudem bieten diese Level eine Denkpause an. Für jeden Racing Level gibt es jedoch die Herausforderung Medaillen zu gewinnen, indem der Level besonders schnell gelöst wird. Die Gold-Medaille ist bei diesen Leveln besonders herausfordernd.

Die Racing Level sind immer die vorletzten Level vor einem Special Theme Level, also 8, 13, 18, 23 und 28.

#### Knobel Level

Ein Knobel Level ist ein „normaler“ Level. Der Spieler muss den Weg zum Exit finden. Mit steigender Levelhöhe wird dies immer schwieriger, da die Levels größer werden und die Level stärker verschachtelt sind. Der Hauptunterschied zu einem Racing Level ist, dass ein Knobel Level weniger Linear ist. Jedoch bieten nur die aller schwersten Level die Möglichkeit sich „frei“ im Level vor und zurück zu bewegen. Der Grad der „nicht-Liniarität“ steigert sich von kleinen Ausbuchtungen und Kanten, in die der Spieler rollen muss, über kurze Weg, die er zurückrollen muss, über sehr kurze Wege, die er auf verschieden Arten lösen kann bis hin zu „Kreuzungen“ die in verschiedene Levelabschnitte führen.

Die Schwierigkeit der Knobel-Levels nimmt zudem mit der Anzahl der Gravityswitches zu. Werds GS in einfachen Levels lediglich zum „Vertuschen“ der Linearität verwendet, wird in späteren Levels verstärkt ein „Umdenken“ notwendig. Es sollten jedoch nie mehr als 2 GS kurz hintereinander auftreten. Im Besondern ist es zu vermeiden, „Knoten“ durch GS zu bauen. Das räumliche Vorstellungsvermögen des Durchschnittsspielers ist stark begrenzt und Orientierungsverlust mindert den Spielspaß.

In einfachen Level ist darauf zu achten, dass die Wege breit sind und dem Spieler ggf. hilfreiche Blockaden aufgebaut werden, die ein häufiges herunterfallen des Balls auch für ungeübte Spieler vermeiden helfen. Geschicklichkeitseinlagen, wie Springen, müssen in einfachen Leveln sehr leicht zu meistern sein. Erst in den letzten 10 Levels dürfen gelegentlich fordernde Passagen auftauchen. Um Frust zu vermeiden sind direkt vor schwierige Passagen Diamanten zu platzieren.

Schalterrätzel sind so zu gestallten, dass die Verbindung zwischen Schalter (Auslöser) und bewegtem Block (Effekt) auch für ungeübte Spieler eindeutig und sofort zu erkennen ist. Ein Schalter muss sich daher in einer Position befinden, in welcher der Effekt direkt beobachtet werden kann. In schwereren Level ist es auch möglich, diese Regel zu brechen, wenn der Effekt durch den logischen Aufbau des Levels zwangsweise nur einem Auslöser zugeordnet werden kann. Ein schwieriger Level enthält maximal 2 Auslöser-Effekt Paare.

#### Bonus Level

Bonus Level sind kleiner als normale Level aber haben viel mehr Diamanten. Ein Bonus Level hat jedoch niemals einen großen Diamanten. Die Levels sollten sehr einfach zu spielen sein. Sie sind als Belohnung gedacht. Zudem sind die Bonus Levels sehr thematisch in ihrem Aufbau(z.B. Blumenform, Tortenform, …). Um die besondere Stellung der Bonuslevel zu unterstreichen, werden als Blocktexturen die alten Blockball V 1.0 Texturen verwendet.

## Leveldesign

Türen sind durch gleichfarbige Blöcke in ihrer Umgebung klar und von weitem zu erkennen. Schlüssel sind ebenfalls durch gleichfarbige Blöcke in ihrer Umgebung gut gekennzeichnet. Die Schlüssel und Tore haben dabei genau zugeordnete Blockfarben, die sich in allen Levels wiederholen. Zudem entspricht die Blockfarbe dem Schlüssel. Ein gelber Schlüssel ist also nicht bei orangenen Blöcken zu finden.

In einfachen Leveln oder in schwierigen Passagen kann der Spieler durch farblich herausstehende „Wege“ geleitet werden. Eine gewählte Blockfarbe führt dabei den Spieler über einen längeren Weg.

Kanten, an denen der Spieler auf gleicher Höhe springen muss, sind gesondert zu kennzeichnen. Dies erfolgt durch das setzte einen 2x2 (oder vergleichbaren) Block an den Kanten, der sich farblich abhebt. Dieser zeigt dem Spieler deutlich an, dass er an dieser Stelle zu springen hat. Der Kantenstreifen ist sowohl eine Unterstützung in der 3D-Sicht des Spielers, als auch ein Mittel zur Spielerführung. Kanten, die zu einer tieferen Ebene führen werden nicht markiert.

 

Markierung notwendig keine Markierung notwendig

Diamanten sind häufig zu platzieren, um ein Neuspielen bereits gespielter Passagen weitgehend zu vermeiden. Zudem sind die Diamanten wichtiger Bestandteil der Spielerführung. Ein Diamant sollt immer in Sichtweite eines vorherigen Diamanten sein. Der Spieler sollte möglichst niemals im Level stehen und nicht weiterwissen, weil er keinen Diamanten sieht. Diamanten sollten aber auch nicht in Massen auftreten. Es sollte mindestens immer ein leeres „Feld“ um einen Diamanten sein. Diese Regel kann gebrochen werden, wenn eine besonders deutliche Spielerführung erforderlich ist.

Diamanten können gut in kleinen Nischen platziert werden und den Spieler mit Sammeln „aufhalten“. In späteren Levels können Diamanten auch an schwer einsehbaren oder schwieriger zu erreichenden Stellen platziert werden. Es ist zudem möglich Diamanten an Stellen zu platzieren, an denen sie erst gesehen werden, wenn der Spieler sie nicht mehr erreichen kann. Dies sollte jedoch die Ausnahme für schwere Levels sein und nur sehr begrenzt eingesetzt werden. In den Tutorial-Levels sind keine „versteckten“ oder schwierigen Diamanten enthalten.

Je Level ist immer genau ein großer Edelstein platziert. Diese befinden sich an versteckten Orten, zu denen kein Diamant oder keine Farbe leitet.

Ganz wichtig ist es darauf zu achten, dass keine Diamanten für den Spieler zu erreichen sind, die er aber gar nicht zu diesem Zeitpunkt erreichen soll. Ein Ausbrechen aus der Spielerführung und ein folgendes Gefühl des „Verlaufen haben´s“ des Spielers ist zu vermeiden. Ebenfalls sehr wichtig ist es dem Spieler stets neue Diamanten auf seinem Weg zu präsentieren. Es ist zu vermeiden, dass der Spieler einen Weg zurückkehren muss, bei dem er schon alle Diamanten aufgesammelt hat und demzufolge orientierungslos ist. Der mögliche Weg ist somit IMMER durch Diamanten JEDERZEIT ersichtlich.

Selbstverständlich darf ein Level keine „Dead Ends“ erhalten. Es ist darauf zu achten, dass der Spieler auf Blöcken landen kann, über die er nur mit großem Geschick (z.B. über zwei folgende 0z8 Blöcke) wieder zurück kann. Die Hoffnung stirbt bekanntlich zu letzt und erfolglos rumhüpfen macht keinen Spaß.

Für kleinere Abschnitte (< 8 Blöcke Distanz) darf es alternative Wege geben \[Referenz: BB „Tutorial 5“\]. Ein Leveldesign darf jedoch nicht so ausgelegt sein, dass grundsätzlich zwei oder mehr Wege zum Ziel führen. Wegekreuzungen sind in schweren Levels möglich. An diesen kann der Spieler entscheiden, welchen Levelabschnitt er als nächstes Spielen möchte. Wichtig ist hierbei, dass er keine „Fehler“ in seiner Wahl machen kann und ggf. größere Teile des Levels „zur Strafe“ neu spielen muss \[Referenz gut: „A Maze Thing“ / Referenz schlecht: „The Edge (ganz alte Version)“\]. Zudem sind Kreuzungen sehr markant und müssen einen hohen Wiedererkennungswert besitzen.

 

Kreuzung OK Kreuzung NICHT OK

Schlüsselrätzel sind möglichst einfach zu halten. Es gibt zu jeder Schlüsselfarbe immer nur einen Schlüssel und ein Schloss. In einfachen und mittleren Levels sind Tür und Schloss sehr dicht beieinander. In mittleren Levels kann es möglich sein, dass der Spieler zwei Schlüssel besitzt und eine Entscheidungsmöglichkeit zwischen zwei Türen hat. Nur in den schwersten Levels können mehrere Schlüssel aufgesammelt werden, die erst später benötigt werden (Referenz für sehr schwer: „A big tree“). Idealerweise wird dem Spieler bereits vor dem Aufsammeln des

Schlüssels die Tür „präsentiert“.

Türen grenzen Level abschnitte voneinander ab. Daher ist es vorteilhaft, wenn der Spieler nach dem benutzen eines Schlüssels nicht mehr die Möglichkeit hat in vorherige Abschnitte zurückzukehren.

Türen und Schlüssel sind durch einen Weg aus farbigen Blöcken miteinander verbunden. Die Blockfarbe entspricht der Schlüssel/Tür Farbe. Dieses Prinzip ist möglichst in allen Levels einzuhalten, aber in leichten und mittleren Levels besonders wichtig (Referenz schwerer Level: „A maze thing“).

Bei der Platzierung der Schlüssel ist darauf zu achten, dass der Spieler „falsche“

Wege gehen kann und somit auch wieder weite „falsche“ Wege zurückkehren kann (Bild ganz links). Ebenfalls soll der Spieler nicht die Möglichkeit haben ins Ziel zu gelangen ohne alle eingesammelten Schlüssel verwenden zu müssen (2. Bild von links). Schlüssel sollen zudem so angeordnet sein, dass der Spieler niemals weite Wege wieder zurückrollen muss (2. Bild von rechts). Ein Level sollte sich bis auf kurze Ausnahmen nicht verzweigen (Bild rechts).





Schlüssel sollten in einfachen Levels immer nah bei der Tür liegen (Bild links).

Mehrere Schlüssel erhöhen den Schwierigkeitsgrad (2. Bild von links). Wenn ein Schlüssel weit entfernt von seinem Tor ist, sollte das Tor beim Schlüssel zumindest sichtbar sein (3. Bild von links). Der Spieler sollte dann auch keine Möglichkeit haben den Schlüssel zu verfehlen oder später wieder zu diesem Punkt zurückrollen zu können. Wenn ein Schlüssel für eine Tür nicht auf dem direkten Weg liegt, so muss er möglichst nahe und offensichtlich in der Nähe der Tür platziert sein (2. Bild von rechts). Nur in den schweren Levels können alternative Routen zum Sammeln der Schlüssel angeboten werden (Bild rechts). Es ist darauf zu achten, dass keine Wege doppelt rollen muss. Bei solchen schwierigen Rätzeln muss der Spieler durch farbliche Wege geführt werden.





Ein Level hat jeweils ein homogenes Design, bzw. Thematik. Die verwendeten Formen orientieren sich dabei an realen Gegenständen und stellen diese in abstrakter Form da. Beispiele sind:

- aztekischer Tempel
- Rennwagen
- Baum
- Murmelbahn

Das Design soll davon ablenken, dass ein Level eigentlich nur aus Blöcken besteht.

Der Spieler soll statt dessen ihm bekannte Formen erkennen. Zudem soll durch eine

Thematik die Levels besser voneinander unterscheidbar sein und dem Spieler länger in Erinnerung bleiben. Zudem lässt sich mir Freunden besser über den

„Raumschifflevel“ als über Level 19 reden.

Das Design muss dabei nicht nur funktional sein, sondern soll bewusst auch verzierende Elemente enthalten. Jedoch dürfen die Zierelemente nicht im Widerspruch zum Spielfluss stehen und sie dürfen auch die Sicht des Spielers nicht übermäßig einschränken.

Aus Blöcken geformten Pfeile sind als Zierelemente nicht zulässig, da diese nicht ausreichend vom Spieler als Hinweis wahrgenommen werden.

Ein schwieriger, aber wichtiger Punkt ist, dass in allen Easy Levels das Ziel bereits am Startpunkt zu sehen ist. Dem ungeübten Spieler muss klar sein, wohin er rollen muss. Das Ziel muss sich gut sichtbar nicht zu weit vor dem Ball in Startblickrichtung befinden. Das Ziel kann dabei in seiner Ausrichtung verdreht sein. In Advanced Levels ist ein „präsentieren“ des Ziels vor dem eigentlichen Erreichen immer wünschenswert.

#### Punktesystem

Erreicht der Spieler in einer bestimmten Zeit das Ziel, so erhält er dafür einen Speed Bonus und somit mehr Punkte. Der Speedbonus trägt wesentlich dazu bei eine gute Highscore zu erhalten. Die Zeiten sind wie folgt definiert:

| Beginner’s Time | unendlich lange – schlechter geht nicht \[braun\] |
| --- | --- |
| Apprentice‘ Time | Die Zeit, die ein mit dem PC halbwegs vertrauter Mensch auf Anhieb schaffen sollte. \[rot\] |
| Journeyman’s Time | Die Zeit, die ein durchschnittlicher Spieler auf Anhieb schaffen sollte. \[blau\] |
| Grand Master’s Time | Zeit in der ein guter Spieler den Level ohne Levelkenntnis auf Anhieb schaffen kann. \[grün\] |
| Mad Genius‘ Time | Zeit in der ein guter Spieler bei Levelkenntnis oder die Entwickler den Level schaffen können. \[goldgelb\] |

Ein geübter Spieler sollte beim ersten Mal Spielen des Spiels nur Journeyman- und ein paar Grand Master-Zeiten erhalten. Die Apprentice-Zeit trennt den ungeübten Spieler von den Spieleleistungen unserer Eltern.

Jede Zeit wird durch eine eigene Farbe repräsentiert. So haben die Zeit-Fenster (der Balken) im Ingame-GUI sowie der Name des Zeit-Fensters bei der Punkte Summary die entsprechende Farbe.



Die Punkte die ein durchschnittlicher Spieler pro Level erreichen kann steigt mit der Höhe des Levels. D.h. dass in späteren Leveles mehr Diamanten zum Punktesammeln im Level vorhanden sein müssen.

Es gibt daher einen Difficulity-Bonus, der mit dem Difficulty-Level ansteigt:

beginner level 50 Punkte easy level 100 Punkte intermediate level 200 Punkte advanced level 400 Punkte hard level 800 Punkte

genius level 1.600 Punkte

Objekte:

Diamant 25 Punkte

| Großer Diamant | 1250 Punkte |
| --- | --- |
| Speed Bonus: |     |
| Genius | 2.500 Punkte |
| Grand Master | 1.250 Punkte |
| Journeyman | 600 Punkte |
| Apprentice | 150 Punkte |
| Beginner | 5 Punkte |
|     |     |

Hat der Spieler alle kleinen Diamanten eingesammelt, so erhält er zudem einen „Diligence Bonus“ von 2500 Punkten.

Je übrig gebliebener Sekunde bis zum Ablaufen der Timebar erhält der Spieler zudem je einen Punkt. Sind also bspw. noch 8 Minuten übrig, so erhält der Spieler

480 Punkte.

Die Spielzeit steigt mit den Leveln ebenfalls an. Die Pro-Zeit beträgt in den Tutorial Levels ca. 1 Minute. Advanced Level sollten ca. 4-8 Minuten benötigen. Expert Level benötigen über 10 Minuten. Die Anzahl der verwendeten Blöcke ist jedoch unabhängig hiervon und vom Leveldesign abhängig. In Blockball 1.0 waren nicht mehr als 1200 Blöcke in einem Level. In Blockball evolution sind nicht mehr als 2500 Blöcke in einem Level möglich.

Jeder Timeframe entspricht einer bestimmten Zeit (z.B. mad genius time) und entspricht bei der Punkteabrechnung einem Pokal. Ist die Timeframe komplett abgelaufen erhält der Spieler keinen Pokal. Die Zeiten sind jedoch so gesetzt, dass der Spieler wirklich grottig schlecht spielen muss, dass die Zeit vollständig abläuft. Die Auszeichnungen für die 5 Plätze sind

1. Pokal: Pokal
2. Pokal: Goldmedaille
3. Pokal: Silbermedaille
4. Pokal: Bronzemedaille
5. Pokal: Glasständer

z.B.:





Die Pokale unterscheiden sich in ihrer Größe, wobei der größte Pokal der wichtigste ist. Zudem unterscheiden sich die Pokale auch in ihrer Form und Farbe, so dass eine Unterscheidung leicht möglich ist. Da für die meisten Spieler der Silberpokal die höchste Auszeichnung sein wird, darf dieser bereits reichlich verziert mit Goldelementen sein.

Die Idee hinter diesem Konzept ist, dass der Spieler immer vom Programm belohnt wird, egal wie gut alle anderen Spieler sind. Zudem sollen auch schlechte Spieler relativ zu ihrer Leistung belohnt werden und so verschiedene Pokale erhalten. Gute Spieler sollen relativ häufig den besten Pokal erhalten und die Herausforderung in Spiel gegen andere Spieler sehen.

Das Spiel belohnt schlechtere Spieler daher hauptsächlich für die gebrauchte Zeit und nicht noch zusätzlich für Vollständigkeit der aufgesammelten Diamanten und somit der erreichten Punktzahl. Das System ist so für schlechte Spieler leichter nachvollziehbar und nicht mehr so kompliziert wie in Blockball V1.0.

Der große Hebel um gute Punktzahlen zu erreichen ist die benötigte Zeit. Somit ist „gutes“ Spielen ziemlich gleich schnelles Spielen. Um jedoch auch im mit dem Profil einen guten Rang erreichen zu können müssen mehrere Durchgänge für einen Level gespielt werden:

1. Kennenlernen der Levels und Lösen der Rätzel
2. Findes des Bonus Diamanten und freischalten der Bounslevel
3. Verbesseren der Punkte bei den Levels, bei denen der Spieler einen schlechten Rang hat

Der Online-Rang ist dann die ultimative Herausforderung, die schnelles Fahren, finden von Bonus-Diamanten und das möglichst vollständige Aufsammeln von Diamanten vereint.

# Level Editor

## Grundlagen

Der Level Editor ist fester Bestandteil des Spiels. Er soll allen Spielern ermöglichen ohne Programmierkenntnisse neue Levels zu erstellen und dies einfach an Freunde weitergeben können. Der Level Editor ist zudem Entwicklungstool mit dem alle neuen Level erstellt werden und alle alten Level nachbearbeitet werden können.

#### Design

Das GUI des Level Editors verwendet das gleiche Farbkonzept, wie das Hauptmenü. Jedoch wird auf den minimalistischen Stiel zugunsten leichterer Zugänglichkeit verzichtet. Zudem kann der Editor nicht per Joypad bedient werden und ist daher auf Tastatur- und Mausbedienung optimiert.

Viele Buttons haben als Inhalt die entsprechende Tastatur-Taste und ein Symbol.

Somit soll es dem Nutzer erleichtert werden Shortcuts zu verwenden. Da Tasten zum Teil modal belegt sind (z.B. w,a,s,d), ändern sich bei Modusänderungen auch die entsprechenden Symbole.



Die Menübar orientiert sich stark an Windows-Standards. Jeder Hauptmenüpunkt und jeder Untermenüpunkt wird durch Mousehoover gehighlighted. Wird ein Hauptmenüpunkt angewählt, bleibt er gehighlighted, so lange entweder ein Untermenüpunkt angewählt wurde oder außerhalb des Menüs geklickt wurde.

Im Mode-Bereich werden alle drei Modi durch die Tasten „1“, „2“ und „3“ repräsentiert. Die Tasten sind beschriftet. Die Taste für den aktuell aktiven Modus wird immer farblich hervorgehoben. Es ist immer einer der drei Modi aktiv. Die Farben für die Modi sind:

1. Select - hellgrün
2. Move - hellblau
3. Rotate - hellrot

Im Cursor-Bereich leuchten die einzelnen Tasten auf, solange sie auf der Tastatur gedrückt werden.

## Menüleiste

Hier können Level geladen und gespeichert sowie Einstellungen zum Level und Leveleditor vorgenommen werden. Alle unten beschriebenen Kontext-Menüs scrollen zügig aus der Menüleiste aus und scrollen beim Schließen wieder nach oben in die Menüleiste ein.

#### New

Mit „new“ wird der bestehende Level gelöscht. Es erscheint eine Sicherheitsabfrage, ob alle ungespeicherten Daten gelöscht werden sollen. Der Button „Cancel“ ist voreingestellt.



#### Load

Mit dem Load-Menü können alle vorhandenen Custom-Levels geladen werden. Die Campaign-Levels stehen nicht zur Verfügung. Die Auswahl erfolgt über eine Liste, die in ihrer Formatierung von Over und Selected States der Blockauswahlliste gleicht. Wie bereits im Hauptmenü wird bereits ein Screenshot des Levels als Preview bereits beim Hoovern über den Level gezeigt. Eine 3D Darstellung wird jedoch nicht geladen. Zudem aktualisiert sich beim Hoovern der Schwierigkeitsgrad des Levels.

Ein Level kann mit dem OK-Button oder auch direkt per Doppelklick gestartet werden.



#### Save

Beim Save Dialog wird auf der rechten Seite eine Zusammenfassung der

Einstellungen gezeigt.

Auf der linken Seite müssen zum Speichern der Levelname und der Name des

Erstellers eingegeben werden. Der Levelname wird auf Vorhandensein bei Druck auf OK geprüft. Ist der Name bereits vorhanden, erscheint ein Pop-Up Fenster, in dem gefragt wird, ob der Level überschrieben werden soll. Wird daraufhin CANCEL gedrückt, ist das Feld „Levelname“ wieder leer.



Verlässt der Spieler mit den Untermenüpunkten Quit oder Main Menü den Editor so wird geprüft, ob es Änderungen am Level gab. Sind Änderungen vorhanden, so erscheint folgendes Pop-Up Fenster (s. „New“).

#### Blockstyle

Hier kann das Blocktextursetting für die Blöcke festgelegt werden. Texturesettings können nicht innerhalb eines Levels gemischt werden. Es wird auf der linken Seite ein Screenshot von einem Level in dem jeweiligen Textureset angezeigt. Für jedes Textureset ist es der gleich Level, so dass eine besser Vergleichbarkeit gegeben ist.



#### Enviroment

Hier kann das Level Setting eingestellt werden. Auf der linken Seite ist jeweils ein Screenshot als Preview zu sehen.



#### Timeframes

Im Untermenü Timeframes können die verschiedenen Zeiten der einzelnen Timeframes für das aktuelle Level eingestellt werden. Für einen neuen Level sind großzügige Zeiten vorgegeben.



Es wird dabei in Minuten und Sekunden als Eingabefeld unterschieden. Der Doppelpunkt ist also immer zu sehen. Wird die Eingabe mit ESC abgebrochen, so erscheint die alte Zahl. Wird eine ungültige Zahl eingegeben, also wenn mit dieser Zeit die Reihenfolge der Zeiten nicht mehr stimmig wäre, so verschwindet die Zahl ebenfalls wieder und es wird der Cursor angezeigt. Zusätzlich wird nun aber die fehlerhafte Differenz in rot dargestellt. Wird eine gültige Zahl angegeben, so erscheint wieder die korrekte Differenz in schwarz.



#### Music

Hier kann das Musikstück für den Level ausgewählt werden. In der Liste werden sowohl alle Campain Musikstücke, als auch alle Musikstücke im Custom-Folder angezeigt. Auf der linken Seite gibt es einen Play-Button. Hiermit kann das gewählte Musikstück (grau hinterlegt) abgespielt werden. Wenn das Musikstück abspielt ändert sich der Button in ein Stop-Symbol und als Text erscheint „Stop“. Die Buttonfarbe für Push ist grün bei „play“ und rot bei „stop“.



#### Set Difficulty

Die Auswahl der Schwierigkeitsstufe erfolgt über eine einfache Liste. Auf der linken Seite steht ein Hinweistext, der den Nutzer darauf hinweist, dass es sich hierbei um seine persönliche Einschätzung handelt.



# Programmaufbau

## Menüstrukturen



## Design

Das Menü unterteilt sich in den Content-Bereich (links) und den Auswahl-Bereich (rechts). Der Content-Bereich stellt das Levels, welches im Hintergrund geladet, dar.

Das Preview stellt nur die Level Blöcke dar, ohne Hintergrund oder Skybox.



Die Menüführung ist immer auf der rechten Seite. Alle Menüpunkte werden immer in kleinen Buchstaben geschrieben. Etwas abgesetzt steht oberhalb immer der Name des Menüs in dem sich der Spieler gerade befindet. In kleinerer Schrift steht direkt darunter der Pfad des aktuellen Menüs ausgehend vom Hauptmenü (z.B. main menu / single / beginner /). Der Pfad wird mit Slash getrennt und endet mit einem Slash.

Um den Pfad möglichst kurz zu halten wird eine Abkürzung des tatsächlichen Menüs verwendet (z.B. statt „play single level“ nur „single“). Ist der Pfad dennoch zu lang wird der Anfang des Pfads durch „… /“ ersetzt. Es werden soviel Pfadnamen durch Pünktchen ersetzt bis der Pfad wieder kurz genug ist.

Die Menüpunkte sind ein Mausbereich, die gehighlighted werden, wenn mit der Maus über den entsprechenden Bereich gehoovert wird. Ein Menüpunkt wird durch den orangenen Kreis (n.n. final!) links neben der Schrift gehighlightet. Der Text ist standard mäßig stark dunkelgrau (90%), gehighlighted ist er schwarz. Wurde eine Selektion gemacht, so dass eine Auswahl im Kontent Bereich erfolgt, so ist dieser gehighlighted. Alle nicht aktiven Menüpunkte im Auswahlbereich sind dann ausgegraut und nicht mit der Maus highlightbar (s. z.B. Optionsmenü). Nur der ausgewählte Menüpunkt ist schwarz.



Die Steuerung des Menüs kann neben der Maus auch über die Tastatur erfolgen. Mit den Pfeiltasten hoch und runter sowie „w“ und „s“ können die Menüpunkte selektiert werden. Mit der Pfeiltaste links sowie „a“ oder ESC kann in einem Untermenü in das nächst höhere Untermenü gesprungen werden (z.B. main menü / single / beginner / main menü / single /). Mit der Pfeiltaste rechts sowie „d“ oder Return kann der Menüpunkt bestätigt werden.

Anmerkung: Die Bedienung für „Menü zurück“ und „Bestätigen“ mit links und rechts gilt nur für das Auswahlmenü. Im Kontextbereich sind die Tasten links und rechts für das Ändern der Einstellungen verwendet.

Wird im Main Menu mit Return der Menüpunkt „Quit“ bestätigt oder im Main Menu die

ESC Taste gedrückt, so wird das Spiel sofort ohne Sicherheitsabfrage beendet. Die Taste „a“ hat im Main Menü keine Funktion.

Jeder Tastendruck oder Hooverwechsel wird auch mit Sound signalisiert (vorerst gleicher Sound wie in Blockball V1.0).

Am Bildschirmrand wird der aktuelle Profilname angezeigt.

## Titelmenü

Das Titelmenü bietet den Einstieg in das Spiel. Der Menüpunkt Start Campaign ist standardmäßig selektiert. Als Pfad wird hier als Platzhalter BlockBall evolution angezeigt. Mit den Tasten Return, „d“ oder Pfeiltaste rechts kann sofort das Spiel an aktueller Stelle gestartet werden. Ein Linksklick mit der Maus auf den Level führt ebenfalls den Campaign an aktueller Stelle fort.



Titelmenü

#### Start Campain (Continue Campain)

Beim ersten Start des Spiels heißt dieser Menüpunkt „Start Campaign“. Ist bereits ein Level in diesem Profil gestartet worden heißt der Menüpunkt „Continue Campaign“.

#### Loading Screen

Der Loading sollte immer nur sehr kurz zu sehen sein, daher ist dieser sehr einfach gehalten. Die Loading Bar wächst während des Ladens des Levels an.



Loading Screen

## Play Single Level

Um die Levels weiter zu unterteilen wird in einem Untermenü zwischen den Campain Levels und den Custom Levels unterschieden.



#### Play Campain Level

Im ersten Menü wird der Schwierigkeitsgrad ausgewählt. Es wird im 3D Preview zunächst immer der zuletzt angewählte Level angezeigt, also z.B. zu Beginn der nächste Campain Level oder eben der gewählte Single Level, wenn dann im Menü zurück gesprungen wurde. Als Menüpunkt können auch die Bonuslevel ausgewählt werden.



Nach dem Auswählen des Schwierigkeitsgrads kann der Level gewählt werden. Auf der rechten Seite des Levels wird ein Schlosssymbol angezeigt, wenn der Level noch nicht freigespielt ist. Wird ein Level angewählt, der noch nicht freigespielt ist wird das Level nicht geladen.



Ist ein Level freigeschalten, so kann dieser jederzeit auch mit Linksklick auf den Levelnamen gestartet werden. Die Tasten Return startet ebenfalls den selektierten Level. Die Tasten „d“ und Pfeiltaste rechts sind hier ohne Funktion.

An dieser Stelle werden bewusst keine Punkte angezeigt. Hier kann der Spieler sich einen Level aussuchen, den er bereits kennt und gerne nochmals spielen möchte. Zudem kann sich hier der Spieler noch unbekannte Level ansehen und einen Vorgeschmack auf spätere Herausforderungen erhalten.

#### Play Custom Level

Wird ein Level gehoovert wird keine Darstellung in 3D erfolgen.

Der Scrollbalken wird nicht immer angezeigt. Die Größe des Balkens ist proportional zur Gesamtgröße der Liste zu der Anzahl der Level die auf einmal auf dem Bildschirm angezeigt werden. Sind weniger wie 6 Levels angezeigt wird der Scrollbalken ausgeblendet. Der Scrollbalken wird niemals kleiner als die Höhe eine Levelkastens, selbst wenn die Liste der Levels sehr viel größer sein sollte.

Wird mit der Maus über einen Level gehoovert, wird die Levelbar gehighlighted und der Menüselektor-Ball verschwindet. Mit Linksklick kann der selektierte Level gestartet werden.

Mit der Taste Return kann über Tastatur in den Kontentbereich gewechselt werden. Die Tasten „d“ und Pfeiltaste rechts sind hier ohne Funktion. Die Levelbar wird wie beim Mousehoover gehighlighted. Mit den Tasten hoch und runter, bzw. „w“ und „s“ kann der Level ausgewählt werden. Der Level kann mit Return gestartet werden. Die Tasten „a“ und „d“ haben im Kontentbereich keine Funktion.



## Optionsmenü

Das Optionsmenü unterteilt sich in die Unterpunkte Camera, Sound, Video und Profil. Wird mit der Maus im Auswahlbereich über einen Menüpunkt gehoovert wird sofort im Kontentbereich der entsprechende Inhalt angezeigt.

Wird mit der Maus über eine Einstellungs-Bar gehoovert wird diese gehighlighted und ist damit aktiv für die Tastatursteuerung. Mit der Tastatur kann der Kontentbereich mit Return gehighlightet werden. Die Tasten „d“ und Pfeiltaste rechts sind hier ohne Funktion, da eine Selektion bestätigt werden muss.

Beim Highlighting verschwinden der Selektor-Ball im Auswahlbereich und alle weiteren Menüpunkte außer dem selektierten werden ausgegraut. Im Kontentbereich wird die oberste Bar farblich hervorgehoben.

Im Kontentbereich kann mit der Tastatur wie bisher nach oben und unten durchselektiert werden. Die Tasten für rechts und links ändern jetzt aber die Einstellungsoption für die jeweils selektierte Einstellungsbar.



In jedem Optionsmenü wird auch der Profilname angezeigt. Bei Profilwechsel werden die sofort die gespeicherten Einstellungen geladen.

#### Camera

Im Controls Menü können wie oben ersichtlich die Einstellungen für die Maus vorgenommen werden. Eine Einstellungsmöglichkeit zum Definieren der Tastatur ist derzeit vorbereitet aber nicht vorgesehen.

### Sound Options

Unter Sound können nur die Lautstärke für Musik und SFX eingestellt werden.



### Video Options

Im Video Menü können die Auflösung und der Texturdetailgrad eingestellt werden. Die Liste der Auflösungen wird von der Grafikkarte bereit gestellt.



Video Menü

### Profil

Wird das Spiel gestartet, so wird zuerst ein Profil namens „BBPlayer“ erstellt. Bei jedem weiteren Start wird immer das zuletzt verwendete Profil geladen. Profile können neu angelegt und gewechselt werden (Menüpunkt „Change Profil“).

Ist bereits ein Profil vorhanden, so wird der höchste freigeschaltene Level geladen. Im Profil sind alle Einstellungen und weitere Daten des Spielers gespeichert (s. Technisches Design Dokument).

Als Spielauflösung wird für die erste Konfiguration die Windows-Auflösung voreingestellt. Die Auflösung wird jedoch nicht im Profil gespeichert.

### Change Profil

Unter diesem Menüpunkt können neue Profile angelegt oder gewechselt werden. Es werden hier keine freigespielte Levels oder Highscores angezeigt.



Change Profile

Wird mit der Maus in den Content-Bereich gehoovert oder mit Return, „d“ oder Pfeiltaste in den Content-Bereich gewechselt, erscheint der in der Namensliste gewählte Name gehighlighted. Mit Return oder Mausklick kann das aktuell gehighlightete Profil geladen werden.

Die oberste Bar der Profile ist die „create new profile“ Bar. Wird diese ausgewählt verschwindet der Text in der Bar und ein Cursor blinkt am linken Rand. Der Nutzer kann dann seinen Namen angeben. Mit Return wird die Eingabe bestätigt, mit ESC wird der Vorgang abgebrochen (es wird kein neues Profil angelegt).



Nach Bestätigung wird ein neues Profil angelegt mit dem im Eingabefeld eingegebenen Namen. Das neue Profil übernimmt alle Optionsmenü-Einstellungen aus dem aktuellen Profil.

Steht kein Text im Eingabefeld erscheint ein neues Pop-Up Fenster on top mit dem Hinweis: „You need to enter a name in the lower text field \[OK-Button\]“. Ist der Name bereits vergeben erscheint die Meldung „Name already in use. Please give yourself a different name.\[OK-button\]. War der Name korrekt wird sofort ins neue Profil gewechselt und das Fenster geschlossen.

Auf der rechten Seite ist der Button für Reset. Der Button hat die Größe ab dem Pfeil bis zum rechten Rand. Ist ein Name gehighlightet kann der Button auch mit „d“ oder den Pfeiltasten rechts ausgewählt werden. Mit dieser Taste kann der Nutzer sein Profil zurücksetzen. Technisch wird das alte Profil gelöscht und ein neues mit gleichem Namen angelegt. Vor dem Reseten erscheint ein neues Pop-up Fenster mit einer Sicherheitsabfrage: „All progress will be lost! Are you sure? \[ok\]\[cancel\]“. Es kann kein Objekt im Hintergrund angewählt werden.

Entsprechend dem Rest Button gibt es den Delete Button auf der Linken Seite. Das löschen eines Profils soll es ermöglichen einen Namen wieder zu entfernen, wenn man sich verschrieben hat. Vor dem Löschen erscheint ein neues Pop-up Fenster mit einer Sicherheitsabfrage: „This will erase all progress made in the campaign with the selected profile and remove the profile from this list! Are you sure you want to delete? \[ok\]\[cancel\]“. Cancel ist dabei vorselektiert. Während das Pop-up Fenster zu sehen ist, ist der Hintergrund leicht ausgebleicht. Es kann kein Objekt im Hintergrund angewählt werden.



# _Hall of Fame_

Die Hall of Fame ist für die fortgeschrittenene Spieler. Während das Programm jeden

Spieler, gute wie schlechte, für Leistungen mit Pokalen belohnt, bietet die Hall of Fame die Herausforderung für alle Spieler die sich gerne mit anderen Spielern messen möchten. Jedes Profil lässt sich so anhand einer einziges Zahl bewerten: am Rang!

Wird die Hall of Fame aufgerufen, werden die ersten 5 Plätze angezeigt. Und an 6. Stelle wird der Player angezeigt mit seiner Platzierung. Wenn der Spieler unter den ersten 5 Plätzen ist wird auch der 6. Platz angezeigt.

Das eigene Profil ist farbig hervorgehoben. Wird mit der Maus in den Kontentbereich gehoovert gibt es kein Highlightning.



Im Auswahlbereich kann der Spieler mit dem obersten Menüpunkt in den „overall“ wechseln. Hier wird die Liste für die gesamte Campaine gezeigt.



Hoovert der Spieler über einen Schwierigkeitsgrad, so wird die Liste weiter gefiltert. Es werden nun nur noch alle Punkte der online Profile betrachtet, die auch in den entsprechenden Levels gemacht wurden. Daraus wird eine eigene Highscoreliste dieser „Liga“ erzeugt.

Der Spieler kann so sein Profil weiter analysieren. Ist er in der Gesamtliste auf Platz 97, in der Beginner-Liga auf Platz 23 und in der Intermediate-Liga auf Platz 154 so weis er bereits wo seine Schwächen und sein Optimierungspotenzial liegt. Er kann nun weiter in die Intermediate-Liga mittels Bestätigung wechseln und seine Analyse auf die einzelnen Level konzentrieren.



In der intermediate Liga kann der Spieler nur mittels hoovern sich die Platzierungen aller internet Profile je Level betrachten. Hat er nun beispielsweise Level 23 als seinen Punktefresser entdeckt kann er wieder per Bestätigung in die Ansicht für diesen Level wechseln.

Grundsätzlich wird für ein Profil immer nur die punktemäßig beste Leistung erfasst.

Hat ein Spieler diesen Level noch nicht freigeschalten, so kann er diesen logischerweise auch noch nicht spielen.



# _Credits Screen_

Die Mitarbeiter scrollen langsam nach oben. Der Credits Screen hat eine eigene Musik.