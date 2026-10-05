const names=['sans-mutations','vitesse','trois-traits','rarefaction','penurie','conflit-maga','grand-monde'];
const labels=['Sans mutations','Mutation vitesse','Trois traits','Raréfaction','Pénurie','Conflit MAGA','Grand monde'];
const resources=['nourriture','eau','terre','minerai','charbon','energie','fer','armes'];
const resourceLabels=['Nourriture','Eau','Terre','Minerai','Charbon','Énergie','Fer','Armes'];
const colors=['#c8941d','#1474b7','#a46529','#747e87','#51455e','#c07f04','#6e8192','#bd3a49'];
const fmt=n=>Number(n).toLocaleString('fr-FR',{maximumFractionDigits:1});
const percent=(n,total)=>fmt(n*100/total)+' %';
const get=(scenario,lot)=>data.find(r=>r.Scenario===scenario&&r.Lot===lot);
const line=(r,key,label,color,dash=false)=>({label,color,dash,values:r.Evolution.map(d=>({x:d.jour,y:d[key]??0}))});
function chart(title,series,limits){
 const width=680,height=255,left=52,right=18,top=18,bottom=34,w=width-left-right,h=height-top-bottom;
 let ymin=limits?.[0]??Math.min(0,...series.flatMap(s=>s.values.map(p=>p.y)));
 let ymax=limits?.[1]??Math.max(1,...series.flatMap(s=>s.values.map(p=>p.y)))*1.05;
 const last=Math.max(...series.flatMap(s=>s.values.map(p=>p.x))),x=d=>left+(d-1)*w/Math.max(1,last-1),y=d=>top+h-(d-ymin)*h/Math.max(.0001,ymax-ymin);
 let svg=`<svg viewBox="0 0 ${width} ${height}" role="img" aria-label="${title}">`;
 for(let i=0;i<=4;i++){const v=ymin+(ymax-ymin)*i/4;svg+=`<line x1="${left}" y1="${y(v)}" x2="${width-right}" y2="${y(v)}" stroke="#e0e7ef"/><text x="${left-7}" y="${y(v)+4}" font-size="11" fill="#586b7e" text-anchor="end">${fmt(v)}</text>`;}
 for(let d of [1,10,20,30,last].filter((v,i,a)=>v<=last&&a.indexOf(v)===i)){svg+=`<text x="${x(d)}" y="${height-12}" font-size="11" text-anchor="middle" fill="#586b7e">J${d}</text>`;}
 for(const s of series){const points=s.values.map(p=>`${x(p.x).toFixed(2)},${y(p.y).toFixed(2)}`).join(' ');svg+=`<polyline fill="none" stroke="${s.color}" stroke-width="2.5" ${s.dash?'stroke-dasharray="6 4"':''} points="${points}"/>`;}
 svg+='</svg>';
 return `<div class="plot"><h3>${title}</h3><div class="legend">${series.map(s=>`<span style="--c:${s.color}">${s.label}${s.dash?' (pointillés)':''}</span>`).join('')}</div>${svg}</div>`;
}
function compareChart(title,key,a,b,limits){return chart(title,[line(a,key,'Avant','#8795a4',true),line(b,key,'Après','#187e91')],limits);}
document.getElementById('comparison').innerHTML='<thead><tr><th>Scénario</th><th>Population finale</th><th>Runs avec raids</th><th>Avec coalitions</th><th>Châteaux endommagés</th><th>Assauts moyens</th></tr></thead><tbody>'+names.map((s,i)=>{const a=get(s,'avant'),b=get(s,'apres');return `<tr><td>${labels[i]}</td><td>${fmt(a.Population)} → ${fmt(b.Population)}</td><td>${percent(a.RunsRaids,a.Runs)} → ${percent(b.RunsRaids,b.Runs)}</td><td>${percent(a.RunsCoalitions,a.Runs)} → ${percent(b.RunsCoalitions,b.Runs)}</td><td>${percent(a.RunsDamage,a.Runs)} → ${percent(b.RunsDamage,b.Runs)}</td><td>${fmt(a.Raids)} → ${fmt(b.Raids)}</td></tr>`;}).join('')+'</tbody>';
document.getElementById('winners').innerHTML='<thead><tr><th>Scénario</th><th>Bleus</th><th>Corail / MAGA</th><th>Verts</th><th>Violets</th><th>Extinction</th><th>Coexistence</th></tr></thead><tbody>'+names.map((s,i)=>{const b=get(s,'apres');return `<tr><td>${labels[i]}</td>${b.Winners.map(n=>`<td>${n}</td>`).join('')}<td>${b.Extinctions}</td><td>${b.Coexistences}</td></tr>`;}).join('')+'</tbody>';
document.getElementById('status').textContent=`${data.reduce((s,r)=>s+r.Runs,0).toLocaleString('fr-FR')} runs vérifiés · ${data.length} lots · ${data[0].Evolution.length} jours par run`;
document.getElementById('scenario').innerHTML=names.map((s,i)=>`<option value="${s}">${labels[i]}</option>`).join('');
function refresh(){
 const scenario=document.getElementById('scenario').value,view=document.getElementById('view').value,a=get(scenario,'avant'),b=get(scenario,'apres');let plots=[];
 document.getElementById('stats').innerHTML=`<div class="stat"><small>Population finale médiane</small><strong>${fmt(b.PopulationMedian)}</strong><small>P10–P90 : ${fmt(b.PopulationP10)}–${fmt(b.PopulationP90)}</small></div><div class="stat"><small>Armes produites / run</small><strong>${fmt(b.WeaponsMade)}</strong></div><div class="stat"><small>Assauts / run</small><strong>${fmt(b.Raids)}</strong></div><div class="stat"><small>Runs avec château à 0 %</small><strong>${percent(b.RunsZero,b.Runs)}</strong></div>`;
 if(view==='combat'){plots=[compareChart('Population vivante','population',a,b),compareChart('Habitants armés','armes_equipees',a,b),compareChart('Assauts cumulés','raids',a,b),compareChart('Attaques de coalition cumulées','coalitions',a,b),compareChart('Défense minimale quotidienne des quatre châteaux','defenses_min',a,b,[0,100]),compareChart('Agrandissements de stockage','reserves',a,b)];}
 if(view==='ressources'){plots=resources.map((r,i)=>chart(resourceLabels[i]+' — stocks des quatre châteaux',[line(a,'stock_'+r,'Avant','#8795a4',true),line(b,'stock_'+r,'Après',colors[i])]));plots.push(...resources.slice(0,6).map((r,i)=>chart(resourceLabels[i]+' — quantité disponible sur la carte',[line(a,'carte_'+r,'Avant','#8795a4',true),line(b,'carte_'+r,'Après',colors[i])])));}
 if(view==='industrie'){plots=[chart('Armes : équipées et stockées',[line(a,'armes_equipees','Équipées avant','#8795a4',true),line(b,'armes_equipees','Équipées après','#156b9b'),line(b,'stock_armes','Stock après','#bd3a49')]),compareChart('Armes fabriquées par soir','production_armes',a,b),compareChart('Fer fabriqué par soir','production_fer',a,b),compareChart('Charbon consommé par soir','consommation_charbon',a,b),compareChart('Nourriture produite par soir','production_nourriture',a,b),compareChart('Énergie produite par soir','production_energie',a,b),compareChart('Agrandissements construits','reserves',a,b),compareChart('Capacité totale par type de ressource','capacite',a,b)];}
 if(view==='phenotypes'){plots=[compareChart('Vitesse moyenne des habitants vivants','vitesse',a,b,[.6,1.4]),compareChart('Taille moyenne des habitants vivants','taille',a,b,[.6,1.4]),compareChart('Perception moyenne des habitants vivants','perception',a,b,[.6,1.4]),chart('Effectifs par faction — après',[line(b,'population_0','Bleus','#287dc1'),line(b,'population_1',scenario==='conflit-maga'?'MAGA':'Corail','#dc7057'),line(b,'population_2','Verts','#598e3e'),line(b,'population_3','Violets','#9356b1')])];}
 if(view==='diplomatie'){plots=[compareChart(`Confiance moyenne envers ${scenario==='conflit-maga'?'MAGA':'Corail'}`,'confiance_corail',a,b,[-100,100]),compareChart(`Nombre d'alliés de ${scenario==='conflit-maga'?'MAGA':'Corail'}`,'allies_corail',a,b,[0,3]),compareChart('Plans de coalition organisés par jour','plans',a,b),compareChart('Attaques de coalition cumulées','coalitions',a,b),compareChart('Assauts cumulés','raids',a,b),compareChart('Défense minimale quotidienne','defenses_min',a,b,[0,100])];}
 document.getElementById('plots').innerHTML=plots.join('');
}
document.getElementById('scenario').addEventListener('change',refresh);document.getElementById('view').addEventListener('change',refresh);refresh();
