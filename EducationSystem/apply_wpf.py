#!/usr/bin/env python3
'''Patch EducationSystem lab-08 without touching existing repositories unrelated to roles.
Run: py -3 tools/apply_wpf.py   (from project root where EducationSystem.slnx is).
Creates timestamped .bak files. Refuses unexpected source layouts.
'''
from pathlib import Path
from datetime import datetime
import re
import sys

root = Path.cwd()
if not (root / 'EducationSystem.slnx').exists():
    print('ПОМИЛКА: відкрий термінал у корені репозиторію (поряд з EducationSystem.slnx).')
    sys.exit(1)
proj=root/'EducationSystem'
changes={}

def read(relative):
    p=proj/relative
    if not p.is_file():
        raise RuntimeError(f'Відсутній файл: {p}')
    return p.read_text(encoding='utf-8-sig')

def replace_one(s, old, new, where):
    if new and new in s:
        return s
    if s.count(old)!=1:
        raise RuntimeError(f'{where}: очікував одне входження, отримав {s.count(old)}. Зміни не застосовано.')
    return s.replace(old,new,1)

# MainWindow: teacher can edit; administration only reads lessons.
p='MainWindow.xaml.cs'; s=read(p)
pattern=r'(LessonsViewModel viewModel\s*=\s*new LessonsViewModel\(\s*lessonService,\s*teacherSubjectService,\s*teacherService,\s*subjectService,\s*groupService)(\s*\);)'
if not re.search(r'groupService,\s*_currentUser.Role == UserRole.Teacher',s):
    s,n=re.subn(pattern, r'\1,\n                _currentUser.Role == UserRole.Teacher,\n                _currentUser.TeacherId\2',s)
    if n!=1: raise RuntimeError(f'{p}: constructor call not found')
changes[p]=s

# LessonsViewModel: read-only admin, teacher own lookup lists only.
p='ViewModels/LessonsViewModel.cs';s=read(p)
s=replace_one(s, 'using System.Collections.ObjectModel;', 'using System.Collections.ObjectModel;\nusing System.Windows;', p)
s=replace_one(s, 'private int? _editingId;', '''private int? _editingId;
        private readonly int? _teacherId;

        public bool CanManageLessons { get; }
        public GridLength EditorColumnWidth =>
            CanManageLessons ? new GridLength(340) : new GridLength(0);
        public GridLength EditorGapWidth =>
            CanManageLessons ? new GridLength(20) : new GridLength(0);''', p)
s=replace_one(s, '''            GroupService groupService)
        {''', '''            GroupService groupService,
            bool canManageLessons,
            int? teacherId)
        {
            CanManageLessons = canManageLessons;
            _teacherId = teacherId;''', p)
s=replace_one(s, '''                TeacherSubjects.Add(item);''', '''                if (!_teacherId.HasValue || item.TeacherId == _teacherId.Value)
                    TeacherSubjects.Add(item);''',p)
s=replace_one(s, '''                Teachers.Add(teacher);''', '''                if (!_teacherId.HasValue || teacher.Id == _teacherId.Value)
                    Teachers.Add(teacher);''',p)
s=replace_one(s, '''        private void New()
        {''', '''        private void New()
        {
            if (!CanManageLessons) return;''',p)
s=replace_one(s, '''        private async Task SaveAsync()
        {''', '''        private async Task SaveAsync()
        {
            if (!CanManageLessons)
            {
                Message = "Адміністрація може лише переглядати заняття.";
                return;
            }''',p)
s=replace_one(s, '''        private async Task DeleteAsync()
        {''', '''        private async Task DeleteAsync()
        {
            if (!CanManageLessons)
            {
                Message = "Адміністрація може лише переглядати заняття.";
                return;
            }''',p)
changes[p]=s

# XAML: remove editor column entirely for administration, expand list.
p='Views/LessonsView.xaml';s=read(p)
s=replace_one(s, '    <Grid Margin="24">', '''    <UserControl.Resources>
        <BooleanToVisibilityConverter x:Key="BoolToVisibility" />
    </UserControl.Resources>

    <Grid Margin="24">''',p)
s=replace_one(s, '<ColumnDefinition Width="20"/>', '<ColumnDefinition Width="{Binding EditorGapWidth}"/>',p)
s=replace_one(s, '<ColumnDefinition Width="340"/>', '<ColumnDefinition Width="{Binding EditorColumnWidth}"/>',p)
s=replace_one(s, '''            <Border
                Grid.Column="2"
                BorderBrush=''', '''            <Border
                Grid.Column="2"
                Visibility="{Binding CanManageLessons, Converter={StaticResource BoolToVisibility}}"
                BorderBrush=''', p)
changes[p]=s

# JournalRepository: route ALL journal reads/writes through scoped dbo procedures.
p='Repositories/JournalRepository.cs';s=read(p)
if 'dbo.usp_GetScopedJournalPage' not in s:
    # The source has a raw string, not a semicolon after the opener.
    m=re.search(r'            const string sql = \x22{3}\s*.*?\s*\x22{3};',s,re.DOTALL)
    if m is None: raise RuntimeError(f'{p}: SQL raw string not found')
    s=s[:m.start()]+'            const string sql = "dbo.usp_GetScopedJournalPage";'+s[m.end():]
    needle='''                    PageSize = pageSize
                }
            );'''
    s=replace_one(s,needle, '''                    PageSize = pageSize
                },
                commandType: CommandType.StoredProcedure
            );''',p)
s=s.replace('"dbo.usp_SaveJournalEntry"', '"dbo.usp_SaveScopedJournalEntry"')
s=s.replace('"dbo.usp_SaveJournalPage"', '"dbo.usp_SaveScopedJournalPage"')
changes[p]=s

# JournalService no longer reads dbo.students directly for teacher; the SQL wrapper
# checks student membership in lesson.group_id and the original save proc validates it.
p='Services/JournalService.cs';s=read(p)
needle='''            Student? student = await _studentRepository.GetByIdAsync(item.StudentId);

            if (student is null)
                throw new ArgumentException("Студента не знайдено.");

            if (student.GroupId != lesson.GroupId)
                throw new ArgumentException("Студент не належить до групи цього заняття.");

'''
if needle in s:
    s=s.replace(needle, '', 1)
elif '_studentRepository.GetByIdAsync(item.StudentId)' in s:
    raise RuntimeError(f'{p}: знайдено нестандартний блок перевірки студента.')
changes[p]=s

# TeacherId must already exist after prior teacher-binding stage.
p='Security/UserSession.cs';s=read(p)
if 'int? TeacherId' not in s:
    raise RuntimeError('UserSession.cs не має TeacherId. Спершу застосуй завершений етап teacher-binding.')

# Source validation before writing any files.
for relative,data in changes.items():
    if not data.strip(): raise RuntimeError(f'Empty resulting file: {relative}')
print('Перевірено:',len(changes),'файлів.')

stamp=datetime.now().strftime('%Y%m%d_%H%M%S')
for relative,data in changes.items():
    path=proj/relative
    original=path.read_text(encoding='utf-8-sig')
    if original==data:
        print('Без змін:',relative); continue
    backup=path.with_name(path.name+f'.{stamp}.bak')
    backup.write_text(original,encoding='utf-8')
    path.write_text(data,encoding='utf-8-sig')
    print('Оновлено:',relative, '| резервна копія:',backup.name)
print('Готово. Виконай dotnet build EducationSystem.slnx')
