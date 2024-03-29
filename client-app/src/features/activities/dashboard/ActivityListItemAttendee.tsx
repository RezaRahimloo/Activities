import { observer } from "mobx-react-lite";
import { List, Image, Popup } from "semantic-ui-react";
import { Profile } from "../../../app/models/profile";
import { Link } from "react-router-dom";
import ProfileCart from "../../profiles/ProfileCart";

interface Props {
    attendees: Profile[];
}

export default observer(function ActivityListItemAttendee({ attendees }: Props) {
   
    return (
        <List horizontal>
            {attendees.map(attendee => (
                <Popup hoverable key={attendee.username} trigger={
                    <List.Item as={Link} to={`/profiles/${attendee.username}`} key={attendee.username}>
                        <Image size="mini" circular src={attendee.image || '/assets/user.png'} />
                    </List.Item>
                } >
                    <Popup.Content>
                        <ProfileCart profile={attendee}/>
                    </Popup.Content>
                </Popup>
                
            ))}
        </List>
    )
});