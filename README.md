# Dokumentacja Techniczna Projektu

Poniższa dokumentacja opisuje architekturę systemów w projekcie. Projekt opiera się na architekturze sterowanej zdarzeniami (Event-Driven), wzorcu Singleton dla głównych menedżerów, architekturze opartej na ScriptableObjects (dla logiki zadań i dialogów) oraz wykorzystuje pakiet Unity New Input System.

---

## 1. Systemy Główne (Core Systems)

### UIManager (`UIManager.cs`)
Centralny zarządca interfejsu użytkownika, zapobiegający konfliktom przy zarządzaniu czasem gry i kursorem.
* **Zasada działania:** Przechowuje zbiory (`HashSet`) paneli pauzujących i niepauzujących. Jeżeli jakikolwiek panel pauzujący jest otwarty, ustawia `Time.timeScale = 0f`. Skrypt wymusza widoczność kursora przez cały czas trwania gry od momentu załadowania sceny.
* **Zastosowanie:** Zamiast operować na `gameObject.SetActive()`, interfejsy powinny używać:
  ```csharp
  UIManager.Instance.OpenPanel(panel, true); // true = pauzuje grę, false = panel w tle (np. powiadomienie)
  UIManager.Instance.ClosePanel(panel);
  UIManager.Instance.TogglePanel(panel, true);
  ```

### DimensionManager (`DimensionManager.cs`)
Statyczna klasa pełniąca rolę globalnego przekaźnika informacji o stanie (wymiarze/kolorze), w którym aktualnie znajduje się gracz.
* **Zasada działania:** Pozwala na odwiązanie skryptów otoczenia od obiektu gracza. Inne skrypty mogą odpytać o stan lub nasłuchiwać jego zmiany.
* **Zastosowanie:**
  * Sprawdzanie stanu: `if (DimensionManager.CurrentState == PlayerColorState.Blue)`
  * Nasłuchiwanie: Należy podpiąć metodę pod `DimensionManager.OnDimensionChanged` w `OnEnable` i obowiązkowo odpiąć ją w `OnDisable`.

### GameManager (`GameManager.cs`)
Globalny kontener przechowujący podstawowe statystyki niezbędne w różnych miejscach projektu.
* **Zawartość:** Przechowuje obiekt `PlayerStatus` (zarządzanie staminą i flagami ruchu gracza) oraz bazowe prędkości dla poruszania się przeciwników.

---

## 2. Mechaniki Gracza

### PlayerMovement (`PlayerMovement.cs`)
Skrypt oparty na komponencie `CharacterController`, odpowiadający za poruszanie się i zarządzanie staminą.
* **Zasada działania:** Pobiera wektory ruchu z New Input System. Redukuje staminę podczas biegu i regeneruje ją podczas marszu/spoczynku. W przypadku spadku staminy do 0, blokuje możliwość biegu do czasu pełnej regeneracji. Oś Y postaci jest zablokowana na stałej wartości.
* **Zależność od dialogów:** Posiada zabezpieczenie sprawdzające stan `DialogueManager.Instance.BlocksMovement`. Jeśli aktywny jest kluczowy dialog, ruch z klawiatury/pada jest całkowicie ignorowany.

### PlayerStateManager (`PlayerStateManager.cs`)
Skrypt zarządzający przełączaniem trzech stanów (White, Blue, Red).
* **Zasada działania:** Zmienia maskę renderowania głównej kamery (`cullingMask`), ukrywając lub pokazując obiekty przypisane do odpowiednich warstw (np. `State_White`). Zmienia również filtr koloru w globalnym Post-Processingu (`Volume`).
* **Integracja z questami:** Posiada opcję podpięcia plików `QuestSO`, które zostaną automatycznie ukończone po pierwszym przełączeniu gracza w dany stan.

### PlayerAnimation (`PlayerAnimation.cs`)
Zarządza animacją i obrotem modelu gracza.
* **Zasada działania:** Zamiast płynnego obrotu, wymusza 8-kierunkowy obrót postaci, przyciągając kąt do najbliższych 45 stopni. Czyta stan ruchu z `GameManager.instance.playerStatus`.

---

## 3. System Interakcji

### PlayerInteractor (`PlayerInteractor.cs`)
System detekcji obiektów interaktywnych w przestrzeni świata (Trigger).
* **Zasada działania:** Tworzy listę obiektów w zasięgu gracza (posiadających interfejs `IInteractable`). Na bieżąco odpytuje `PlayerStateManager`, czy dany obiekt znajduje się na aktualnie widocznej warstwie.
* **Podświetlanie:** Klonuje materiał najbliższego widocznego obiektu i modyfikuje jego parametr `_EmissionColor`, mnożąc kolor bazowy przez wartość natężenia.
* **Oczyszczanie:** Posiada mechanizm usuwający z listy obiekty usunięte w trakcie gry (`Destroy`), co zapobiega błędom braku referencji. Zamyka okno dialogowe, jeśli gracz opuści trigger w trakcie rozmowy (o ile rozmowa na to pozwala).

### IInteractable (`IInteractable.cs`)
Interfejs, który musi posiadać każdy interaktywny obiekt na scenie. Wymaga zaimplementowania metod `OnInteract`, `GetInteractText` oraz `GetTransform`.

---

## 4. System Zadań (Quest System)

### QuestSO (`QuestSO.cs`)
* Reprezentacja pojedynczego zadania jako ScriptableObject.
* Przechowuje tytuł, opis, stan odblokowania/ukończenia oraz referencję do zadania poprzedzającego (`previousQuest`), co pozwala na automatyczne tworzenie łańcuchów zadań.

### QuestManager (`QuestManager.cs`)
Singleton zarządzający stanem wszystkich zadań w grze.
* **Zasada działania:** Posiada globalną listę `allQuests`. Odpowiada za zmianę stanu zadań i wywoływanie zdarzeń `OnQuestUnlocked` oraz `OnQuestCompleted`. Wymaga przypisania klipów audio do odgrywania przy zmianie stanu zadań.

### QuestJournalUI (`QuestJournalUI.cs`)
Interfejs dziennika zadań.
* **Zasada działania:** Reaguje na akcję z Input Systemu. Dzieli zadania na kolumny: zablokowane (z opcją ukrycia szczegółów przed graczem), aktywne i zakończone. Posiada mechanikę wibracji przycisku w interfejsie, informującą gracza o nowym, nieprzeczytanym zadaniu.

---

## 5. System Dialogów

### DialogueSO (`DialogueSO.cs`)
* ScriptableObject definiujący strukturę rozmowy.
* Zawiera zmienną `canWalkAway` (definiującą, czy gracz może przerwać dialog odchodząc od NPC), listę kwestii dialogowych (rozmówca, portret, tekst, klip audio) oraz listę opcji wyboru dla gracza. Wybory mogą prowadzić do kolejnego pliku `DialogueSO` lub odblokowywać określony `QuestSO`.

### DialogueManager (`DialogueManager.cs`)
Singleton obsługujący mechanikę rozmów.
* **Zasada działania:** Otwiera panel UI bez pauzowania gry (`UIManager.Instance.OpenPanel(..., false)`). Obsługuje efekt pisania na maszynie (Typewriter effect) bazujący na czasie rzeczywistym. Tworzy interaktywne przyciski dla opcji wyboru. Pozwala na siłowe zamknięcie okna przez `PlayerInteractor`, jeśli gracz opuści strefę rozmowy.

---

## 6. Mini-gry / UI Komputera

### ComputerManager (`ComputerManager.cs` i `PCInteractable.cs`)
* Skrypt `PCInteractable` służy jako punkt styku w świecie gry. Po aktywacji otwiera instancję `ComputerManager`.
* `ComputerManager` zawiera system własnych paneli (logowanie, pulpit, aplikacje). Po wykonaniu odpowiedniej sekwencji w minigrze, komunikuje się z `QuestManager` celem zakończenia powiązanego zadania i uaktualnia obiekty na scenie.

---

## 7. System Audio

### AudioManager (`AudioManager.cs`)
Singleton zarządzający muzyką i efektami dźwiękowymi.
* **Zasada działania:** Skrypt nasłuchuje zdarzenia `OnDimensionChanged` z klasy `DimensionManager`. W przypadku zmiany, uruchamia proces płynnego przenikania (Cross-fade). Zapisuje pozycję odtwarzania (`Timestamp`) aktualnego utworu, ścisza go, podmienia na utwór powiązany z nowym stanem i wznawia odtwarzanie od zapisanego momentu, co gwarantuje zachowanie tempa utworu.

---

## 8. Dobre Praktyki w Projekcie

1. **Zarządzanie Pamięcią:** Każde subskrybowanie zdarzenia (np. `+= HandleUpdate`) wymaga odpowiedniego odpięcia (`-= HandleUpdate`) w metodzie `OnDisable` lub `OnDestroy`.
2. **Sterowanie:** Omijanie klasy `Input` (np. `Input.GetKeyDown`). Do obsługi wejścia należy używać wyłącznie akcji zdefiniowanych w pliku konfiguracyjnym Input Action Asset i wywoływać je przez zdarzenie `performed`.
3. **Czas i Kursor:** Bezpośrednia modyfikacja zmiennej `Time.timeScale` oraz `Cursor.lockState` jest zabroniona. Zmiany te muszą być realizowane wyłącznie poprzez rejestrowanie paneli w obiekcie `UIManager`.
