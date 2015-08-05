using System;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;

namespace ITJakub.ITJakubService.Core
{
    public class NotesManager
    {
        private readonly NoteRepository m_noteRepository;
        private UserRepository m_userRepository;

        public NotesManager(UserRepository userRepository, NoteRepository noteRepository)
        {
            m_userRepository = userRepository;
            m_noteRepository = noteRepository;
        }

        public void CreateNote(NoteRepository notesRepository)
        {
                        
        }

        public void CreateNote(string note, int? userId)
        {
            User user = null;
            if(userId.HasValue)
                user = m_userRepository.Load<User>(userId);

            Note entity = new Note {CreateDate = DateTime.UtcNow, Text = note, User = user};
            m_noteRepository.Save(entity);
        }
    }
}