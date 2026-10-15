import { useEffect, useState } from "react";

export type Clip = {
        title: string
        id: string
};

export async function isDaemonRunning() {
        const response = await fetch("http://localhost:5000/api/status");

        return(response.ok);
}

export function useClips() {
        const [clips, setClips] = useState<Clip[]>([]);

        useEffect(() => {
                async function fetchClips() {
                        const newClips = [...clips];

                        const response = await fetch("http://localhost:5000/api/getClips");
                        const fetchedClips = await response.json();

                        for (let clipId of fetchedClips) {
                                const clip = {title: "Placeholder clip title", id: clipId} as Clip;

                                newClips.push(clip);
                        }

                        setClips(newClips);
                }

                // 

                fetchClips();
        }, []);

        return { clips };
}
