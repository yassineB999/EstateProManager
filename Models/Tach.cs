﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EstateProManager.Models;

public partial class Tach
{
    public long Id { get; set; }

    public long IdDemande { get; set; }

    public long IdPrestataire { get; set; }

    public string DetailsTache { get; set; }

    public string StatutTache { get; set; }

    public virtual Demande IdDemandeNavigation { get; set; }

    public virtual Utilisateur IdPrestataireNavigation { get; set; }
}