# Statistiques de survie en pénurie

Expérience réalisée sur le moteur actuel, sans modifier les règles du jeu. Nourriture distribuée : 10 par jour ; agriculture, stocks, eau, relief, chef et diplomatie actifs dans le scénario principal. Graines 1001 à 1002, au maximum 3 jours par partie. Les scénarios de contrôle utilisent les mêmes graines.

**Dernier individu** : premier instant observé après un pas où un seul individu est encore vivant. Il peut ensuite mourir ; on mesure qui reste en dernier, pas son immortalité. **Dernière faction** : première faction seule encore vivante, même avec plusieurs habitants. Une extinction simultanée ne reçoit aucun dernier individu. Les parties qui atteignent 3 jours sans individu unique sont censurées ; elles peuvent avoir déjà une faction seule.

## Jeu actuel : 25 fondateurs

2 simulations.

| Faction | Dernier individu | % des simulations | Intervalle à 95 % | Dernière faction |
|---|---:|---:|---|---:|
| Bleus | 0 | 0.0 % | 0.0–65.8 % | 0 |
| MAGA | 0 | 0.0 % | 0.0–65.8 % | 0 |
| Verts | 0 | 0.0 % | 0.0–65.8 % | 0 |
| Violets | 0 | 0.0 % | 0.0–65.8 % | 0 |

Extinctions sans dernier individu unique : 0. Sans dernier individu après 3 jours : 2. Sans dernière faction unique observée : 2.

## Lecture du code et limites

- La graine par défaut est 42. « Recommencer » reproduit la même expérience avec les mêmes règles : les relances ne sont pas indépendantes. Pour ce lot, chaque partie possède une graine différente.
- Avec 25 fondateurs distribués par i % 4, les Bleus commencent à 7, les autres à 6. Les Bleus disposent notamment d'un artisan supplémentaire. Avec 24, toutes les factions commencent à 6 avec la même répartition de métiers.
- La carte fixe ne présente pas de symétrie entre ouest, nord et sud : les lacs, montagnes et dépôts affectent les trajets, l'eau et les métiers. L'opposition initiale aux MAGA est une autre différence ; les côtés de départ sont retirés aléatoirement les jours suivants.
- Les traits suivent la même distribution initiale, mais les individus possèdent des génomes différents ; un petit échantillon ne garantit pas les mêmes phénotypes, sexes ou descendants par faction.
- Le commerce traite les paires de factions dans un ordre fixe, et la planification des raids parcourt les cibles par indice. Des effets de cet ordre sont possibles ; le scénario de permutation aide à distinguer les positions des noms de faction sans isoler chaque mécanisme.
- La permutation conserve les mêmes organismes, gènes, sexes, métiers, positions et carte, et change seulement les étiquettes culturelles des trois factions non-MAGA : ouest devient Vert, nord Violet, sud Bleu. Les MAGA restent à l'est. Les réserves initiales de ces trois camps sont identiques.
- Le contrôle neutre désactive tout le petit monde : pas de relief fonctionnel, eau, chef, réserves ni raids. Il vérifie la symétrie du modèle de base ; il ne représente pas un monde avec des MAGA particuliers.
- Le contrôle sans raids conserve le relief, les ressources, le chef et l'avidité de collecte, mais désactive les raids et les alliances.

Les CSV contiennent un enregistrement par graine, la faction du dernier individu, les deux marqueurs de peau, les traits, le métier, le sexe, le jour, les extinctions et les effectifs restants à la limite. Les XML donnent les règles exactes. `version-moteur.csv` conserve les empreintes SHA-256 des sources. Exécution : `powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Simuler-Penurie.ps1`.
