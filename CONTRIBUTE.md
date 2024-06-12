# When Commit

## When Push to develop
```bash
git update-index --assume-unchanged src/Apps/Corathing.Organizer/Resources/CorathingOrganizerLocalizationStringResources.Designer.cs
git update-index --assume-unchanged src/Widgets/Corathing.Widgets.Basics/Resources/BasicWidgetStringResources.Designer.cs
```

## When Push to main or Resource Updated
```bash
git update-index --no-assume-unchanged src/Apps/Corathing.Organizer/Resources/CorathingOrganizerLocalizationStringResources.Designer.cs
git update-index --no-assume-unchanged src/Widgets/Corathing.Widgets.Basics/Resources/BasicWidgetStringResources.Designer.cs
```