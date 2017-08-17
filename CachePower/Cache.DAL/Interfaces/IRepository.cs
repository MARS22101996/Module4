﻿using Cache.DAL.Entities;

namespace Cache.DAL.Interfaces
{
    public interface IRepository
    {
        Cargo GetById(int id);

        void Create(Cargo item);
    }
}