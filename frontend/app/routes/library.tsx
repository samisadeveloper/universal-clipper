import type { Route } from "./+types/library";
import { isDaemonRunning, useClips, type Clip } from "../ClipLoader";
import ClipPreview from "../components/ClipPreview";
import { useEffect, useState } from "react";
import Header from "../components/Header";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Viewing clips in the library" },
    { name: "description", content: "Download the client to manage clips" },
  ];
}



const secondaryText = '#b6b6b8';

export default function Library() {
        const { clips } = useClips();

        const [ currentClip, setClip ] = useState<Clip>();
        const [isRunning, setRunning] = useState(false);

        useEffect(() => {
                async function getStatus() {
                        const running = await isDaemonRunning();

                        setRunning(running);
                }

                getStatus();
        });

        return (
                <div>
                        <Header />

                        <div style={{justifyItems: 'center'}}>
                                {clips.length > 0 && 
                                        <div style={{
                                                display: "grid",
                                                gridTemplateColumns: "auto auto auto"
                                        }}>
                                        {clips.map(clip => (
                                                <ClipPreview key={clip.id} clip={clip} onClickVideo={() => {
                                                        setClip(clip);
                                                }}/>
                                        ))}
                                </div>
                                }
                                {clips.length < 1 && 
                                        <div style={{alignContent: 'center', justifyItems: 'center', height: '100vh'}}>
                                                <div style={{justifyItems: 'center', width: 1000, height: 500}}>
                                                        <h1 style={{marginTop: 20, fontSize: 55}}>Waiting for the magic to happen</h1>
                                                        <p style={{color: secondaryText, fontSize: 35}}>{isRunning ? "No clips detected so far." : "No clipper client running."}</p>
                                                </div>
                                        </div>
                                }
                        </div>
                </div>
        );
}
