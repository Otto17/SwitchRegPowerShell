**SwitchRegPowerShell** - Простая консольная программа для удобного изменения политики запуска скриптов в PowerShell.

Утилита предназначена для упрощения автоматизации и администрирования, что бы разрешать запуск скриптов PowerShell непосредственно перед выполнением сценария, после запрещать выполнение скриптов для сохранения повышенной безопасности системы.

_Данная программа является свободным программным обеспечением, распространяющимся по лицензии MIT._

---

_**Что может данная утилита:**_

*   Установить параметр политики запуска PowerShell скриптов для системы
*   Восстановить политику запуска скриптов “По умолчанию”
*   Вывести политику исполнения скриптов

---

_**Программа принимает только один аргумент:**_

```plaintext
Restricted   - запрещает запуск всех скриптов, кроме тех, которые введены напрямую в PowerShell или предварительно загружены.
AllSigned    - разрешает выполнение только подписанных скриптов.
RemoteSigned - разрешает выполнение всех локальных скриптов, а для удаленных требует подписи.
Bypass       - игнорирует все ограничения и разрешает выполнение всех скриптов.
Unrestricted - разрешает выполнение всех скриптов без ограничений (уровень полномочий выше, чем у "Bypass").
List         - выводит список всех установленных политик выполнения скриптов в PowerShell
Off          - Восстанавливает политику "По умолчанию
```

---

### Внимание! Программа должна быть запущена с правами администратора!

---

_**Примеры.**_

_Команда_:

```plaintext
SwitchRegPowerShell.exe List
```

Выведет:

```plaintext
       Scope ExecutionPolicy
       ----- ---------------
MachinePolicy      Undefined
  UserPolicy       Undefined
     Process       Undefined
 CurrentUser       Undefined
LocalMachine       Undefined
```

_Команда_:

```plaintext
SwitchRegPowerShell.exe Bypass
```

Выведет:

```plaintext
Значение "ExecutionPolicy" изменено на "bypass".
```

_Команда_:

```plaintext
SwitchRegPowerShell.exe Off
```

Выведет:

```plaintext
Параметр "ExecutionPolicy" был удален.
```

**Запуск программы без аргументов или с неверным аргументом выведет справку.**

---

Для демонстрации запуска скрипта запустите из папки “**TEST**”  файл “**Проверка.bat**”.

Для разрешения выполнения скриптов запустите “**ON.bat**”, для запрета выполните “**OFF.bat**”.

---

**Автор Otto, г. Омск 2024**
