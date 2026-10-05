# Statistiques de survie en pÃ©nurie

ExpÃ©rience rÃ©alisÃ©e sur le moteur actuel, sans modifier les rÃ¨gles du jeu. Nourriture distribuÃ©e : 10 par jour ; agriculture, stocks, eau, relief, chef et diplomatie actifs dans le scÃ©nario principal. Graines 0 Ã  999, au maximum 200 jours par partie. Les scÃ©narios de contrÃ´le utilisent les mÃªmes graines.

**Dernier individu** : premier instant observÃ© aprÃ¨s un pas oÃ¹ un seul individu est encore vivant. Il peut ensuite mourir ; on mesure qui reste en dernier, pas son immortalitÃ©. **DerniÃ¨re faction** : premiÃ¨re faction seule encore vivante, mÃªme avec plusieurs habitants. Une extinction simultanÃ©e ne reÃ§oit aucun dernier individu. Les parties qui atteignent 200 jours sans individu unique sont censurÃ©es ; elles peuvent avoir dÃ©jÃ  une faction seule.

## Jeu actuel : 25 fondateurs

10 simulations.

| Faction | Dernier individu | % des simulations | Intervalle Ã  95 % | DerniÃ¨re faction |
|---|---:|---:|---|---:|
| Bleus | 1 | 10.0 % | 1.8â€“40.4 % | 1 |
| MAGA | 4 | 40.0 % | 16.8â€“68.7 % | 5 |
| Verts | 1 | 10.0 % | 1.8â€“40.4 % | 1 |
| Violets | 2 | 20.0 % | 5.7â€“51.0 % | 2 |

Extinctions sans dernier individu unique : 0. Sans dernier individu aprÃ¨s 200 jours : 2. Sans derniÃ¨re faction unique observÃ©e : 1.
Parmi les 4 derniers individus non-MAGA : Bleu 25.0 %, Vert 25.0 %, Violet 50.0 %. Test des trois frÃ©quences Ã©gales : chiÂ² = 0.500, 2 degrÃ©s de libertÃ©, p = 0.7788.
Ce test porte sur les parties rÃ©solues par un individu unique. Les parties censurÃ©es limitent la portÃ©e de cette comparaison ; les scÃ©narios sont des essais du mÃªme moteur, pas des observations biologiques.
Jour moyen du dernier individu : 92.1. Derniers individus Ã  peau mixte : 2/8. La faction culturelle et la peau ne sont pas la mÃªme mesure.

| Dernier individu de la faction | Vitesse moyenne | Taille moyenne | Perception moyenne |
|---|---:|---:|---:|
| Bleus | 1.007 | 0.866 | 0.995 |
| MAGA | 1.020 | 0.983 | 0.954 |
| Verts | 0.988 | 0.950 | 1.054 |
| Violets | 1.007 | 0.999 | 1.071 |

## Lecture du code et limites

- La graine par dÃ©faut est 42. Â« Recommencer Â» reproduit la mÃªme expÃ©rience avec les mÃªmes rÃ¨gles : les relances ne sont pas indÃ©pendantes. Pour ce lot, chaque partie possÃ¨de une graine diffÃ©rente.
- Avec 25 fondateurs distribuÃ©s par i % 4, les Bleus commencent Ã  7, les autres Ã  6. Les Bleus disposent notamment dâ€™un artisan supplÃ©mentaire. Avec 24, toutes les factions commencent Ã  6 avec la mÃªme rÃ©partition de mÃ©tiers.
- La carte fixe ne prÃ©sente pas de symÃ©trie entre ouest, nord et sud : les lacs, montagnes et dÃ©pÃ´ts affectent les trajets, lâ€™eau et les mÃ©tiers. Lâ€™opposition initiale aux MAGA est une autre diffÃ©rence ; les cÃ´tÃ©s de dÃ©part sont retirÃ©s alÃ©atoirement les jours suivants.
- Les traits suivent la mÃªme distribution initiale, mais les individus possÃ¨dent des gÃ©nomes diffÃ©rents ; un petit Ã©chantillon ne garantit pas les mÃªmes phÃ©notypes, sexes ou descendants par faction.
- Le commerce traite les paires de factions dans un ordre fixe, et la planification des raids parcourt les cibles par indice. Des effets de cet ordre sont possibles ; le scÃ©nario de permutation aide Ã  distinguer les positions des noms de faction sans isoler chaque mÃ©canisme.
- La permutation conserve les mÃªmes organismes, gÃ¨nes, sexes, mÃ©tiers, positions et carte, et change seulement les Ã©tiquettes culturelles des trois factions non-MAGA : ouest devient Vert, nord Violet, sud Bleu. Les MAGA restent Ã  lâ€™est. Les rÃ©serves initiales de ces trois camps sont identiques.
- Le contrÃ´le neutre dÃ©sactive tout le petit monde : pas de relief fonctionnel, eau, chef, rÃ©serves ni raids. Il vÃ©rifie la symÃ©trie du modÃ¨le de base ; il ne reprÃ©sente pas un monde avec des MAGA particuliers.
- Le contrÃ´le sans raids conserve le relief, les ressources, le chef et lâ€™aviditÃ© de collecte, mais dÃ©sactive les raids et les alliances.

Les CSV contiennent un enregistrement par graine, la faction du dernier individu, les deux marqueurs de peau, les traits, le mÃ©tier, le sexe, le jour, les extinctions et les effectifs restants Ã  la limite. Les XML donnent les rÃ¨gles exactes. `version-moteur.csv` conserve les empreintes SHA-256 des sources. ExÃ©cution : `powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Simuler-Penurie.ps1`.
