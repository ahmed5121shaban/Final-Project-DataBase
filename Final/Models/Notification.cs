﻿using Final.Enums;
using Final.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zaeid.models;

namespace finalproject.Models
{


    public class Notification
    {
        public int Id { get; set; }
        public Enums.NotificationType Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsReaded { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

    }

    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {

        }
    }
}
