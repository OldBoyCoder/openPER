Make codes
The MAKES table has four codes:

- F = Fiat
- I = Innocenti
- L = Lancia
- R = ALfa Romeo

`select distinct (MK_COD) from MODELS` gives three makes F, L and R
`select distinct (MK_COD) from CATALOGUES` gives three makes F, L and R
`select distinct (MK2_COD) from CATALOGUES` gives four makes:
- F
- L
- R
- T

The 'T' group seems to indicate the commercial vans.  Is there a narrative for these anywhere though?

The only other table that MK2_COD appears on is `COMM_MODGROUP` which gives CMG_COD and CMG_DSC for a model code and description.

Row counts may help

- CATALOGUES  161
- COMM_MODGRP 60
- COMM_MODELS 210
- MODELS 103 

Looking at COMM_MODGRP
- Fiat - 26 - Only 24 appear in ePER
- Alfa - 15
- Lancia - 11
- Commercial - 8

This SQL tries the appropriate JOINS


